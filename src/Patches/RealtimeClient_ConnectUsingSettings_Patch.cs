using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Il2CppPhoton.Realtime;
using Il2CppPhoton.Client;
using System.Collections;
using UnityEngine;

namespace ShrimpleNetworkingAPI.Patches;

[DecompilerComment("In photon realtime you only recieve infomation for rooms in the same lobby, so what im doing here is as soon as possible I join the 'Modded' Lobby")]
[HarmonyPatch(typeof(RealtimeClient), nameof(RealtimeClient.ConnectUsingSettings))]
public static class RealtimeClient_ConnectUsingSettings_Patch
{
    public static void Postfix(Il2CppPhoton.Realtime.RealtimeClient __instance, bool __result, Il2CppPhoton.Realtime.AppSettings __0)
    {
        MelonCoroutines.Start(joinLobbyCoroutine());
        void setLocalModList()
        {
            PhotonHashtable localRootTable = new();
            PhotonHashtable localCustomProperties = new();
            PhotonHashtable localPersonalMods = new();

            localRootTable[Utils.HashTableKeys.InstalledMods] = localPersonalMods;

            __instance.LocalPlayer.CustomProperties[Utils.HashTableKeys.ShrimpleRootNode] = localRootTable;

            foreach (var modkvp in Registration.RegisteredMods)
            {
                localPersonalMods[modkvp.Key] = modkvp.Value.Version.ToString();
                if (modkvp.Value.CustomPropertiesForPlayer.Count is not 0)
                {
                    PhotonHashtable customTable = new();
                    localCustomProperties[modkvp.Key] = customTable;
                    foreach (var customProp in modkvp.Value.CustomPropertiesForPlayer)
                    {
                        customTable[customProp.Key] = customProp.Value;
                    }
                }
            }
        }
        IEnumerator joinLobbyCoroutine()
        {
            int iterationGuard = 100 * 60;
            while (iterationGuard >= 0)
            {
                yield return new WaitForSeconds(1);
                iterationGuard--;
                if (__instance.IsConnectedAndReady)
                {
                    setLocalModList();
                    __instance.OpJoinLobby(new("ShrimpleNetworkingAPI Modded (https://github.com/blankochan/ShrimpleNetworkingAPI)", LobbyType.Default));
                    yield break;
                }
                if (iterationGuard <= 0) yield break;

            }
            if (iterationGuard is <= 0)
                Melon<Main>.Logger.Warning("Could not load into Modded Lobby in 100 seconds");
        }
    }
}
