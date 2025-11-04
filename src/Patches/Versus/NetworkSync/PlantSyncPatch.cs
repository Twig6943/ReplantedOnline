using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.NetworkSync;

[HarmonyPatch]
internal static class PlantSyncPatch
{
    [HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
    [HarmonyPrefix]
    internal static bool Die_Prefix(Plant __instance)
    {
        if (InternalCallContext.IsInternalCall_Die) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.ZombieSide) return false;

            __instance.GetNetworkedPlant()?.SendDieRpc();
            __instance.DieOriginal();

            return false;
        }

        return true;
    }

    internal static void DieOriginal(this Plant __instance)
    {
        InternalCallContext.IsInternalCall_Die = true;
        try
        {
            __instance.Die();
        }
        finally
        {
            // Always reset the flag, even if an exception occurs
            InternalCallContext.IsInternalCall_Die = false;
        }
    }

    /// <summary>
    /// Thread-safe context flags to prevent infinite recursion when calling patched methods from within patches.
    /// [ThreadStatic] ensures each thread has its own copy of these flags.
    /// </summary>
    private static class InternalCallContext
    {
        [ThreadStatic]
        public static bool IsInternalCall_Die;
    }
}
