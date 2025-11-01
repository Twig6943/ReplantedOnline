using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppTekly.PanelViews;
using MelonLoader;
using ReplantedOnline.Network.Online;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ReplantedOnline.Patches.UI;

[HarmonyPatch]
internal static class PanelViewContainerPatch
{
    [HarmonyPatch(typeof(PanelViewContainer), nameof(PanelViewContainer.Awake))]
    [HarmonyPostfix]
    internal static void Awake_Postfix(PanelViewContainer __instance)
    {
        if (!NetLobby.IsInLobby()) return;

        var VsSideChooser = __instance.m_panels.FirstOrDefault(pan => pan.gameObject.name == "P_VsSideChooser");
        if (VsSideChooser != null)
        {
            if (NetLobby.IsLobbyHost())
            {
                VsSideChooser.RemoveVSButton("Custom");
                VsSideChooser.SetVSButton("QuickPlay", () =>
                {
                    RPC.SendStartGame(SelectionSet.QuickPlay);
                });
                VsSideChooser.SetVSButton("CustomAll", () =>
                {
                    RPC.SendStartGame(SelectionSet.CustomAll);
                });
                VsSideChooser.SetVSButton("Random", () =>
                {
                    RPC.SendStartGame(SelectionSet.Random);
                });
            }
            else
            {
                VsSideChooser.RemoveSelectionButtons();
            }
        }
    }

    private static void SetVSButton(this PanelView panelView, string name, Action callback)
    {
        MelonCoroutines.Start(CoSetVSButton(panelView, name, callback));
    }

    private static IEnumerator CoSetVSButton(this PanelView panelView, string name, Action callback)
    {
        yield return new WaitForSeconds(1f);
        var button = panelView.transform.Find($"Canvas/Layout/Center/Panel/SelectionSets/{name}")?.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick = new();
            button.onClick.AddListener(callback);
        }
    }

    private static void RemoveVSButton(this PanelView panelView, string name)
    {
        var button = panelView.transform.Find($"Canvas/Layout/Center/Panel/SelectionSets/{name}")?.gameObject;
        if (button != null)
        {
            UnityEngine.Object.Destroy(button);
        }
    }

    private static void RemoveSelectionButtons(this PanelView panelView)
    {
        var buttons = panelView.transform.Find($"Canvas/Layout/Center/Panel/SelectionSets")?.gameObject;
        if (buttons != null)
        {
            UnityEngine.Object.Destroy(buttons);
        }
    }
}
