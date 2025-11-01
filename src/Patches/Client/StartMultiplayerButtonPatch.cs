using HarmonyLib;
using Il2CppSource.Binders;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Patches.Client;

[HarmonyPatch]
internal class StartMultiplayerButtonPatch
{
    [HarmonyPatch(typeof(StartMultiplayerButton), nameof(StartMultiplayerButton._onButtonClicked))]
    [HarmonyPrefix]
    internal static bool _onButtonClicked_Prefix()
    {
        NetLobby.CreateLobby();
        return false;
    }
}
