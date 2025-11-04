using HarmonyLib;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus.NetworkSync;

[HarmonyPatch]
internal static class ZombieSyncPatch
{
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.PlayDeathAnim))]
    [HarmonyPrefix]
    internal static bool BoardAddCoin_Prefix(Zombie __instance, DamageFlags theDamageFlags)
    {
        if (NetLobby.AmInLobby())
        {
            if (VersusState.PlantSide) return false;

            __instance.GetNetworkedZombie()?.SendDeathRpc(theDamageFlags);
            __instance.PlayDeathAnimOriginal(theDamageFlags);
        }

        return true;
    }

    internal static void PlayDeathAnimOriginal(this Zombie __instance, DamageFlags theDamageFlags)
    {
        InternalCallContext.IsInternalCall_PlayDeathAnim = true;
        try
        {
            __instance.PlayDeathAnim(theDamageFlags);
        }
        finally
        {
            // Always reset the flag, even if an exception occurs
            InternalCallContext.IsInternalCall_PlayDeathAnim = false;
        }
    }

    /// <summary>
    /// Thread-safe context flags to prevent infinite recursion when calling patched methods from within patches.
    /// [ThreadStatic] ensures each thread has its own copy of these flags.
    /// </summary>
    private static class InternalCallContext
    {
        [ThreadStatic]
        public static bool IsInternalCall_PlayDeathAnim;
    }
}
