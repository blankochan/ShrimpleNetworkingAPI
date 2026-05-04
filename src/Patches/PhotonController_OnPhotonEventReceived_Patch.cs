using HarmonyLib;
using Il2CppPhoton.Client;
using Il2Cpp;

namespace ShrimpleNetworkingAPI.Patches;

[DecompilerComment("Hide rooms with mismatched mods")]
[HarmonyPatch(typeof(PhotonController), nameof(PhotonController.OnPhotonEventReceived))]
public static class PhotonController_OnPhotonEventReceived_Patch
{
    public static void Postfix(Il2Cpp.PhotonController __instance, Il2CppPhoton.Client.EventData __0) // spaggeti code qwq
    {
        if (__0.Code is 229)
        {
            PhotonHashtable rooms = __0[222].Cast<PhotonHashtable>();
            foreach (var room in rooms)
            {
                string roomName = room.Key.ToString();
                PhotonHashtable roomProperties = room.Value.Cast<PhotonHashtable>();

                if (Utils.TryGetRequiredMods(roomProperties, out var remoteRoomRequiredMods))
                {

                    foreach (var remoteModKvp in remoteRoomRequiredMods)
                    {
                        if (Registration.RegisteredMods.ContainsKey(remoteModKvp.Key))
                        {
                            var localMod = Registration.RegisteredMods[remoteModKvp.Key];
                            if (localMod.CustomRejector is not null)
                            {
                                var rejectorOut = localMod.CustomRejector.Invoke(roomProperties);
                                if (rejectorOut.Reject)
                                    removeRoom(roomName, rejectorOut.Reason);
                            }
                            if (localMod.UseStrictVersioning)
                                if (remoteModKvp.Value != localMod.Version)
                                {
                                    removeRoom(roomName, $"Rejected because {remoteModKvp.Key} uses StrictVersioning the local client is {localMod.Version} while the room is {remoteModKvp.Value}");
                                    break;
                                }
                        }
                        else
                        {
                            removeRoom(roomName, $"Rejected because the local client does not have {remoteModKvp.Key}");
                            break;
                        }
                    }
                }
                else removeRoom(roomName, "Rejected because this room is missing a valid mod list");

                void removeRoom(string name, string reason = "Unspecifed")
                {
                    // TODO instead of removing the room it should grey out, and then give a way to see the reason why (PRS WELCOME!!!)
                    // could also be refactored to instead of removing the room instantly, queue the removals up so instead of the user seeing the first reason for rejection they see all of the reasons it was rejected
                    if (__instance.availableRooms.ToArray().Any(room => room.name == name))
                    {
                        __instance.availableRooms.Remove(__instance.availableRooms.ToArray().First(room => room.name == name));
                        __instance.PushSimpleEvent(PhotonControllerEventType.RoomsUpdated);
                    }
                }
            }
        }
    }
}
