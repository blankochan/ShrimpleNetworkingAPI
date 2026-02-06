using HarmonyLib;
using Newtonsoft.Json;
using Il2CppPhoton.Realtime;

namespace ShrimpleNetworkingAPI.Patches;

[HarmonyPatch(typeof(RealtimeClient), nameof(RealtimeClient.OpCreateRoom))]
public static class CreateRoom_Patch
{
    public static void Prefix(Il2CppPhoton.Realtime.RealtimeClient __instance, bool __result, Il2CppPhoton.Realtime.EnterRoomArgs __0)
    {
        __0.RoomOptions.CustomRoomProperties.Add("ShrimpleNetworkingAPI_RequiredMods", JsonConvert.SerializeObject(Registration.RegisteredModsRequiredToJoin));
        __0.RoomOptions.CustomRoomPropertiesForLobby = __0.RoomOptions.CustomRoomPropertiesForLobby.Append("ShrimpleNetworkingAPI_RequiredMods").ToArray();
    }
}
