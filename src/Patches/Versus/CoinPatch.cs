using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus;

[HarmonyPatch]
internal static class CoinPatch
{
    [HarmonyPatch(typeof(Board), nameof(Board.AddCoin))]
    [HarmonyPrefix]
    private static bool BoardAddCoin_Prefix(CoinType theCoinType)
    {
        // Only handle network synchronization when in a multiplayer lobby
        if (NetLobby.AmInLobby())
        {
            if (theCoinType is (CoinType.VersusTrophyPlant or CoinType.VersusTrophyZombie))
            {
                return false;
            }

            if (theCoinType == CoinType.Sun && VersusState.AmZombieSide)
            {
                return false;
            }
            else if (theCoinType == CoinType.Brain && VersusState.AmPlantSide)
            {
                return false;
            }
        }

        // Not in lobby - allow normal coin creation
        return true;
    }
}
