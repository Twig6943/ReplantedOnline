using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppSource.DataModels;
using ReplantedOnline.Managers;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Versus;

[HarmonyPatch]
internal static class VersusModePatch
{
    // TODO: Classes to look at to sync actions, Ahhhhhh I HATE Il2Cpp :(
    // VersusMode : ReloadedMode
    // VersusDataModel : DisposableObjectModel
    // VersusPlayerModel : DisposableObjectModel
    // VersusChooserSwapBinder : Binder
    // VersusWinDataResetActivity : InjectableActivity
    // Board : Widget
    // GameplayActivity : InjectableActivity
    // SeedChooserScreen : Widget

    [HarmonyPatch(typeof(VersusMode), nameof(VersusMode.InitializeGameplay))]
    [HarmonyPostfix]
    private static void InitializeGameplay_Postfix()
    {
        VersusManager.OnStart();
    }

    // Stop game from placing initial sunflower in vs
    [HarmonyPatch(typeof(Board), nameof(Board.AddPlant))]
    [HarmonyPrefix]
    private static bool AddPlant_Prefix()
    {
        if (NetLobby.AmInLobby() && Instances.GameplayActivity.VersusMode.m_versusTime < 1f)
        {
            return false;
        }

        return true;
    }

    // Stop game from placing initial gravestones in vs
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IZombiePlaceZombie))]
    [HarmonyPrefix]
    private static bool IZombiePlaceZombie_Prefix(ZombieType theZombieType)
    {
        if (NetLobby.AmInLobby() && Instances.GameplayActivity.VersusMode.m_versusTime < 1f)
        {
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(VersusPlayerModel), nameof(VersusPlayerModel.Confirm))]
    [HarmonyPostfix]
    private static void Confirm_Postfix(VersusPlayerModel __instance)
    {
        if (!NetLobby.AmLobbyHost()) return;

        if (Instances.GameplayActivity.VersusMode.PlantPlayerIndex == 0)
        {
            NetLobby.LobbyData.Networked.HostIsOnPlantSide = true;
        }
        else
        {
            NetLobby.LobbyData.Networked.HostIsOnPlantSide = false;
        }
    }

    [HarmonyPatch(typeof(VersusPlayerModel), nameof(VersusPlayerModel.Cancel))]
    [HarmonyPostfix]
    private static void Cancel_Postfix(VersusPlayerModel __instance)
    {
        if (!NetLobby.AmLobbyHost()) return;

        NetLobby.LobbyData.Networked.PickingSides = true;
    }
}
