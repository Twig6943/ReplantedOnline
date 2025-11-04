using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.NetworkSync;

[HarmonyPatch]
internal static class LawnMowerSyncPatch
{
    [HarmonyPatch(typeof(LawnMower), nameof(LawnMower.StartMower))]
    [HarmonyPrefix]
    internal static bool StartMower_Prefix(LawnMower __instance)
    {
        if (InternalCallContext.IsInternalCall_StartMower) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.ZombieSide) return false;

            __instance.StartMowerOriginal();
        }

        return true;
    }

    internal static void StartMowerOriginal(this LawnMower __instance)
    {
        InternalCallContext.IsInternalCall_StartMower = true;
        try
        {
            __instance.StartMower();
        }
        finally
        {
            // Always reset the flag, even if an exception occurs
            InternalCallContext.IsInternalCall_StartMower = false;
        }
    }

    /// <summary>
    /// Thread-safe context flags to prevent infinite recursion when calling patched methods from within patches.
    /// [ThreadStatic] ensures each thread has its own copy of these flags.
    /// </summary>
    private static class InternalCallContext
    {
        [ThreadStatic]
        public static bool IsInternalCall_StartMower;
    }
}
