using HarmonyLib;
using Il2CppReloaded.UI;
using ReplantedOnline.Helper;
using ReplantedOnline.Network.Online;
using UnityEngine.UI;

namespace ReplantedOnline.Patches.UI;

[HarmonyPatch]
internal static class GameplayOptionsMenuPatch
{
    [HarmonyPatch(typeof(GameplayOptionsMenu), nameof(GameplayOptionsMenu.OnEnable))]
    [HarmonyPostfix]
    private static void OnEnable_Postfix(GameplayOptionsMenu __instance)
    {
        // Only modify the menu if we're in an online lobby
        if (NetLobby.AmInLobby())
        {
            var restartLevelButton = __instance.transform.Find("P_OptionsPanel_Canvas/Layout/Center/Panel/Bottom/Buttons/Hlayout/P_BasicButton_RestartLevel")?.GetComponentInChildren<Button>(true);
            if (NetLobby.AmLobbyHost())
            {
                restartLevelButton.onClick = new();
                restartLevelButton.onClick.AddListener(() =>
                {
                    NetLobby.LobbyData?.Networked?.ResetLobby();
                });
            }
            else
            {
                UnityEngine.Object.Destroy(restartLevelButton.gameObject);
            }

            // Replace main menu button with lobby leave functionality
            var mainMneuButton = __instance.transform.Find("P_OptionsPanel_Canvas/Layout/Center/Panel/Bottom/Buttons/Hlayout/P_BasicButton_MainMenu")?.GetComponentInChildren<Button>(true);
            mainMneuButton.onClick = new();
            mainMneuButton.onClick.AddListener(() => NetLobby.LeaveLobby());
        }
    }
}