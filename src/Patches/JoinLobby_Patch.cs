using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Il2CppPhoton.Realtime;
using System.Collections;
using UnityEngine;

namespace ShrimpleNetworkingAPI.Patches;

[DecompilerComment("In photon realtime you only recieve infomation for rooms in the same lobby, so what im doing here is as soon as possible I join the 'Modded' Lobby")]
[HarmonyPatch(typeof(RealtimeClient), nameof(RealtimeClient.ConnectUsingSettings))]
public static class JoinLobbyPatch
{
    public static void Postfix(Il2CppPhoton.Realtime.RealtimeClient __instance, bool __result, Il2CppPhoton.Realtime.AppSettings __0)
    {
        MelonCoroutines.Start(joinLobbyCoroutine());
        void setLocalModList()
        {
            __instance.LocalPlayer.CustomProperties.Add("ShrimpleNetworkingAPI_Mods", JsonConvert.SerializeObject(Registration.RegisteredMods));
        }
        IEnumerator joinLobbyCoroutine()
        {
            int iterationGuard = 100;
            while (iterationGuard >= 0)
            {
                yield return new WaitForSeconds(1);
                if (__instance.IsConnectedAndReady && __instance.IsConnected && __instance.CurrentLobby.IsDefault)
                {
                    if (__instance.OpJoinLobby(new("Modded", LobbyType.Default)))
                    {
                        setLocalModList();
                        break;
                    }
                }
                iterationGuard--;

            }
            if (iterationGuard is <= 0)
                Melon<Main>.Logger.Warning("Could not load into Modded Lobby in 100 seconds");
        }
    }
}
