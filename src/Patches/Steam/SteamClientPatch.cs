using HarmonyLib;
using Il2CppSteamworks;
using ReplantedOnline.Enums;

namespace ReplantedOnline.Patches.Steam;

[HarmonyPatch]
internal class SteamClientPatch
{
    [HarmonyPatch(typeof(SteamClient), nameof(SteamClient.Init))]
    [HarmonyPrefix]
    private static void SteamClient_Init_Prefix(ref uint appid)
    {
        appid = (uint)AppType.Space_War; // User Space War P2P servers
    }
}
