using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using ReplantedOnline.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ReplantedOnline.Modules;

internal static class ReplantedOnlinePopup
{
    private static PanelView Panel;
    private static TextMeshProUGUI Header;
    private static TextMeshProUGUI SubText;
    private static TextMeshProUGUI Label;
    private static bool _hasInit;

    internal static void Init(PanelViewContainer globalPanels)
    {
        if (_hasInit) return;
        _hasInit = true;

        Panel = UnityEngine.Object.Instantiate(globalPanels.transform.Find("P_PopUpMessage02").GetComponentInChildren<PanelView>(true), globalPanels.transform);
        Panel.name = "P_PopUpReplantedOnline";
        Panel.m_id = "rPopup";
        Panel.gameObject.DestroyAllTextLocalizers();
        Header = Panel.transform.Find("Canvas/Layout/Center/Window/HeaderText").GetComponentInChildren<TextMeshProUGUI>(true);
        Header.gameObject.DestroyAllBinders();
        SubText = Panel.transform.Find("Canvas/Layout/Center/Window/SubheadingText").GetComponentInChildren<TextMeshProUGUI>(true);
        SubText.gameObject.DestroyAllBinders();
        var button = Panel.transform.Find("Canvas/Layout/Center/Window/Buttons/P_BacicButton_Ok").GetComponentInChildren<Button>(true);
        button.gameObject.DestroyAllBinders();
        button.gameObject.SetActive(true);
        Label = button.transform.Find("Label").GetComponentInChildren<TextMeshProUGUI>(true);
        Label.SetText(string.Empty);
        button.onClick = new();
        button.onClick.AddListener(() =>
        {
            SetButtonLabel(string.Empty);
            Hide();
        });
    }

    internal static void SetButtonLabel(string label)
    {
        Label?.SetText(label);
    }

    internal static void Show(string header, string text)
    {
        if (Label?.text == string.Empty)
        {
            Label.SetText("Ok");
        }
        Panel?.gameObject.SetActive(true);
        Header?.SetText(header);
        SubText?.SetText(text);
    }

    internal static void Hide()
    {
        Panel?.gameObject.SetActive(false);
        Header?.SetText(string.Empty);
        SubText?.SetText(string.Empty);
    }
}
