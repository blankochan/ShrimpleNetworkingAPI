using Newtonsoft.Json;
using Il2CppPhoton.Client;
namespace ShrimpleNetworkingAPI;

[DecompilerComment("for now the only thing registration does is hide rooms you dont have the required mods for")]
public static class Registration
{
    private static Dictionary<string, NetworkingMetadata> _registeredMods = new();

    public static IReadOnlyDictionary<string, NetworkingMetadata> RegisteredMods => _registeredMods;
    public static IReadOnlyDictionary<string, NetworkingMetadata> RegisteredModsRequiredToJoin => _registeredMods.Where(mod => mod.Value.RequiredForJoin).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


    public static bool TryRegister(NetworkingMetadata info)
    {
        if (_registeredMods.ContainsKey(info.Identifer)) return false;
        _registeredMods.Add(info.Identifer, info);
        return true;
    }

    public static bool TryRegister(string identifer, string version = "0.0.0", bool requiredForJoin = true)
      => TryRegister(new() { Identifer = identifer, Version = version, RequiredForJoin = requiredForJoin });

#pragma warning disable CS8625
    /// <summary>All of the metadata for ShrimpleNetworkingAPI Registration</summary>
    /// <remarks>Currently as of v0.1.0 the only reason to mess with this would be to setup a custom rejector</remarks>
    public class NetworkingMetadata
    {
        public string Identifer = "Unknown";
        public string Version = "0.0.0";
        public bool RequiredForJoin = true;
        public bool UseStrictVersioning = false;

        /// <summary>Allows for Custom Rejection.</summary>
        /// <returns>Tuple with a boolean determining if it should reject aswell as the reason for rejection.</returns>
        /// <remarks> An example of applicable Use case would be a Custom map mod hiding rooms the user does not have the required maps for.</remarks>
        [JsonIgnore]
        public Func<PhotonHashtable, (bool Reject, string Reason)> CustomRejector = null;

        [JsonIgnore]
        /// <remarks>Ultimately turned into a PhotonHashTable</remarks>
        public Dictionary<string, Il2CppSystem.Object> CustomPropertiesForRoom = new();

        [JsonIgnore]
        /// <remarks>Ultimately turned into a PhotonHashTable</remarks>
        public Dictionary<string, Il2CppSystem.Object> CustomPropertiesForPlayer = new();

        // TODO Easy data sync 
        // with like a callback system and probably message pack for serialization???
        // Will need to figure out an unused event code and whatnot

        public override string ToString()
        {
            return $"{Identifer}|{Version}|{RequiredForJoin}|{UseStrictVersioning}";
        }
    }
}
