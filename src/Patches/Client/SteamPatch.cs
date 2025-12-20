using HarmonyLib;
using Il2CppSteamworks;
using ReplantedOnline.Managers;

namespace ReplantedOnline.Patches.Client;

[HarmonyPatch]
internal class SteamPatch
{
    [HarmonyPatch(typeof(SteamClient), nameof(SteamClient.Init))]
    [HarmonyPrefix]
    private static void SteamClient_Init_Prefix(ref uint appid)
    {
        BloomEngineManager.InitMelon();
        appid = BloomEngineManager.m_gameServer.Value;
    }
}
