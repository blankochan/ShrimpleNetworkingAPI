using HarmonyLib;
using Il2CppPhoton.Client;
using Il2Cpp;

namespace ShrimpleNetworkingAPI.Patches;

[DecompilerComment("Hide rooms with mismatched mods")]
[HarmonyPatch(typeof(PhotonController), nameof(PhotonController.OnPhotonEventReceived))]
public static class PhotonController_OnPhotonEventReceived_Patch
{
    public static void Postfix(Il2Cpp.PhotonController __instance, Il2CppPhoton.Client.EventData __0)
    {
        if (__0.Code is 229)
        {
            void removeRoom(string name)
            {
                __instance.availableRooms.Remove(__instance.availableRooms.ToArray().First(room => room.name == name));
                __instance.PushSimpleEvent(PhotonControllerEventType.RoomsUpdated);
            }
            PhotonHashtable rooms = __0[222].Cast<PhotonHashtable>();
            foreach (var room in rooms)
            {
                string roomName = room.Key.ToString();
                PhotonHashtable roomProperties = room.Value.Cast<PhotonHashtable>();

                Dictionary<string, Registration.NetworkingInfo> remoteRoomRequiredMods = new();
                if (!Utils.TryGetMods(roomProperties, key: "ShrimpleNetworkingAPI_RequiredMods", out remoteRoomRequiredMods))
                    removeRoom(roomName);

                foreach (var remoteModKvp in remoteRoomRequiredMods)
                {
                    if (Registration.RegisteredMods.ContainsKey(remoteModKvp.Key))
                    {
                        var localMod = Registration.RegisteredMods[remoteModKvp.Key];
                        if (localMod.UseStrictVersioning)
                            if (remoteModKvp.Value.Version != localMod.Version)
                            {
                                removeRoom(roomName);
                                break;
                            }
                    }
                    else
                    {
                        removeRoom(roomName);
                        break;
                    }
                }
            }
        }
    }
}
