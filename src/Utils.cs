using Il2Cpp;
using Newtonsoft.Json;
using Il2CppPhoton.Realtime;
using Il2CppPhoton.Client;
namespace ShrimpleNetworkingAPI;

public static class Utils
{
    public static bool TryGetRequiredModsForCurrentRoom(out Dictionary<string, Registration.NetworkingInfo> mods) =>
      TryGetRequiredMods(PhotonController.instance.client.CurrentRoom, out mods);

    public static bool TryGetMods(this PhotonHashtable table, string key, out Dictionary<string, Registration.NetworkingInfo> mods)
    {
        if (table.TryGetValue(key, out var Il2CppModListObject))
        {
            var requiredMods = JsonConvert.DeserializeObject<Dictionary<string, Registration.NetworkingInfo>>(Il2CppModListObject.ToString());
            if (requiredMods is not null)
            {
                mods = requiredMods;
                return true;
            }
        }

        mods = new();
        return false;
    }

    public static bool TryGetRequiredMods(this Room room, out Dictionary<string, Registration.NetworkingInfo> mods)
    {
        if (TryGetMods(room.customProperties, "ShrimpleNetworkingAPI_RequiredMods", out mods))
            return true;

        mods = new();
        return false;
    }


    public static bool IsAvailableInCurrentRoom(this Registration.NetworkingInfo info)
    {
        if (Utils.TryGetRequiredModsForCurrentRoom(out var mods))
            return mods.ContainsKey(info.Identifer);

        return false;
    }

}
