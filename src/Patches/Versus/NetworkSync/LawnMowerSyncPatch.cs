using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.RPC.Handlers;

namespace ReplantedOnline.Patches.Versus.NetworkSync;

[HarmonyPatch]
internal static class LawnMowerSyncPatch
{
    [HarmonyPatch(typeof(LawnMower), nameof(LawnMower.MowZombie))]
    [HarmonyPrefix]
    internal static bool StartMower_Prefix(LawnMower __instance, Zombie theZombie)
    {
        if (InternalCallContext.IsInternalCall_StartMower) return true;

        if (NetLobby.AmInLobby())
        {
            if (VersusState.PlantSide) return false;

            __instance.MowZombieOriginal(theZombie);
            MowZombieHandler.Send(__instance.Row, theZombie.GetNetworkedZombie());
        }

        return true;
    }

    internal static void MowZombieOriginal(this LawnMower __instance, Zombie theZombie)
    {
        InternalCallContext.IsInternalCall_StartMower = true;
        try
        {
            __instance.MowZombie(theZombie);
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
