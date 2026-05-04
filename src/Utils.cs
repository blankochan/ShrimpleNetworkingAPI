using Il2Cpp;
using Newtonsoft.Json;
using Il2CppPhoton.Realtime;
using Il2CppPhoton.Client;
namespace ShrimpleNetworkingAPI;


public static class Utils
{
    public static class HashTableKeys
    {
        public const string ShrimpleRootNode = "ShrimpleNetworkingAPI";
        public const string RequiredMods = "RequiredMods";
        public const string InstalledMods = "InstalledMods";
        public const string CustomProperties = "CustomProperties";
    }


    public static bool TryGetRequiredModsForCurrentRoom(out Dictionary<string, string> mods) =>
      TryGetRequiredMods(PhotonController.instance.client.CurrentRoom.CustomProperties, out mods);


    public static bool TryGetRequiredMods(this PhotonHashtable roomTable, out Dictionary<string, string> mods)
    {
        if (roomTable.TryGetValue(HashTableKeys.ShrimpleRootNode, out var table))
        {
            if (table.Cast<PhotonHashtable>().TryGetValue(HashTableKeys.RequiredMods, out var il2cppModTable))
            {
                Dictionary<string, string> modList = new();
                foreach (var modkvp in il2cppModTable.Cast<PhotonHashtable>())
                {
                    string id = modkvp.Key.ToString();
                    string version = modkvp.Value.ToString();
                    modList.Add(id, version);
                }
                mods = modList;
                return true;
            }
        }

        mods = new();
        return false;
    }

    /// <remarks> <see langword="implicit"/>ly assumes that if <paramref name="rootTable"/> contains HashTableKeys.ShrimpleRootNode that you wanted the <paramref name="customProperties"/> for that root node </remarks>
    public static bool TryGetCustomProperties(PhotonHashtable rootTable, out PhotonHashtable customProperties)
    {
        if (rootTable.TryGetValue(HashTableKeys.ShrimpleRootNode, out var table))
            rootTable = table.Cast<PhotonHashtable>();

        if (rootTable.TryGetValue(Utils.HashTableKeys.CustomProperties, out var customTable))
        {
            customProperties = customTable.Cast<PhotonHashtable>();
            return true;
        }
        customProperties = new();
        return false;
    }
    public static bool IsAvailableInCurrentRoom(this Registration.NetworkingMetadata info)
    {
        if (Utils.TryGetRequiredModsForCurrentRoom(out var mods))
            return mods.ContainsKey(info.Identifer);

        return false;
    }

}
