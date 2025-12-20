using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Object.Game;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.Plants;

[HarmonyPatch]
internal static class PotatominePlantPatch
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    [HarmonyPrefix]
    private static bool FindTargetZombie_Prefix(Plant __instance)
    {
        if (__instance.mSeedType is not SeedType.Potatomine) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.AmZombieSide)
            {
                return false;
            }
        }

        return true;
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    [HarmonyPostfix]
    private static void FindTargetZombie_Postfix(Plant __instance, ref Zombie __result)
    {
        if (__instance.mSeedType is not SeedType.Potatomine) return;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.AmPlantSide)
            {
                var netPlant = __instance.GetNetworked<PlantNetworked>();
                if (__result != null)
                {
                    netPlant.SendPotatomineRpc(__result);
                }
                else
                {
                    if (netPlant._State is Zombie zombie)
                    {
                        __result = zombie;
                    }
                }
            }
        }
    }
}
