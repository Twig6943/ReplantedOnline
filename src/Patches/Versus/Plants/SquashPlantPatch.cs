using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Object.Game;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.Plants;

[HarmonyPatch]
internal static class SquashPlantPatch
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindSquashTarget))]
    [HarmonyPrefix]
    private static bool FindSquashTarget_Prefix(Plant __instance, ref Zombie __result)
    {
        if (__instance.mSeedType is not SeedType.Squash) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.AmZombieSide)
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.FindSquashTarget))]
    [HarmonyPostfix]
    private static void FindSquashTarget_Postfix(Plant __instance, Zombie __result)
    {
        if (__instance.mSeedType is not SeedType.Squash) return;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.AmPlantSide)
            {
                if (__result != null)
                {
                    var netPlant = __instance.GetNetworked<PlantNetworked>();
                    {
                        netPlant.SendSquashRpc(__result);
                    }
                }
            }
        }
    }
}
