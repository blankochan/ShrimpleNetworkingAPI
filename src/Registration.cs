namespace ShrimpleNetworkingAPI;

[DecompilerComment("for now the only thing registration does is hide rooms you dont have the required mods for")]
public static class Registration
{
    private static Dictionary<string, NetworkingInfo> _registeredMods = new();

    public static IReadOnlyDictionary<string, NetworkingInfo> RegisteredMods => _registeredMods;
    public static IReadOnlyDictionary<string, NetworkingInfo> RegisteredModsRequiredToJoin => _registeredMods.Where(mod => mod.Value.RequiredForJoin).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


    public static bool Register(NetworkingInfo info)
    {
        if (_registeredMods.ContainsKey(info.Identifer)) return false;
        _registeredMods.Add(info.Identifer, info);
        return true;
    }

    public static bool Register(string identifer, string version = "0.0.0", bool requiredForJoin = true)
      => Register(new(identifer, version, requiredForJoin));

    public class NetworkingInfo
    {
        public string Identifer;
        public string Version = "0.0.0";
        public bool RequiredForJoin = true;
        public bool UseStrictVersioning = false;

        // TODO Easy data sync 
        // with like a callback system and probably message pack for serialization???
        // Will need to figure out an unused event code and whatnot

        public override string ToString()
        {
            return $"{Identifer}|{Version}|{RequiredForJoin}|{UseStrictVersioning}";
        }

        public NetworkingInfo(string identifer, string version = "0.0.0", bool requiredForJoin = true, bool strictVersioning = false)
        {
            Identifer = identifer;
            Version = version;
            RequiredForJoin = requiredForJoin;
            UseStrictVersioning = strictVersioning;
        }
    }
}
