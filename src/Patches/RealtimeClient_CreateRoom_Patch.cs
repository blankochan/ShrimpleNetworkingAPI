using HarmonyLib;
using Newtonsoft.Json;
using Il2CppPhoton.Client;
using Il2CppPhoton.Realtime;

namespace ShrimpleNetworkingAPI.Patches;

[HarmonyPatch(typeof(RealtimeClient), nameof(RealtimeClient.OpCreateRoom))]
[HarmonyPriority(1000)]
public static class RealtimeClient_CreateRoom_Patch
{
    public static void Prefix(Il2CppPhoton.Realtime.RealtimeClient __instance, bool __result, Il2CppPhoton.Realtime.EnterRoomArgs __0)
    {
        PhotonHashtable shrimpleRoot = new();
        PhotonHashtable shrimpleCustomProperties = new();
        PhotonHashtable shrimpleRequiredMods = new();

        shrimpleRoot[Utils.HashTableKeys.RequiredMods] = shrimpleRequiredMods;
        shrimpleRoot[Utils.HashTableKeys.CustomProperties] = shrimpleCustomProperties;
        foreach (var kvp in Registration.RegisteredModsRequiredToJoin)
        {
            shrimpleRequiredMods[kvp.Key] = kvp.Value.Version.ToString();
            if (kvp.Value.CustomPropertiesForRoom.Count is not 0)
            {
                PhotonHashtable customTable = new();
                shrimpleCustomProperties[kvp.Key] = customTable;
                foreach (var customProp in kvp.Value.CustomPropertiesForRoom)
                {
                    customTable[customProp.Key] = customProp.Value;
                }
            }
        }

        __0.RoomOptions.CustomRoomProperties.Add(Utils.HashTableKeys.ShrimpleRootNode, shrimpleRoot);
        __0.RoomOptions.CustomRoomPropertiesForLobby = __0.RoomOptions.CustomRoomPropertiesForLobby.Append(Utils.HashTableKeys.ShrimpleRootNode).ToArray();
    }
}
