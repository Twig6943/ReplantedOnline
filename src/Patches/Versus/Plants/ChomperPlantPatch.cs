using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.Plants;

[HarmonyPatch]
internal static class ChomperPlantPatch
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    [HarmonyPrefix]
    private static bool Find_Prefix(Plant __instance)
    {
        if (__instance.mSeedType is not SeedType.Chomper) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.AmZombieSide)
            {
                return false;
            }
        }

        return true;
    }
}