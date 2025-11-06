using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.Localizations;
using UnityEngine;

namespace ReplantedOnline.Helper;

internal static class ObjectHelper
{
    internal static void DestroyAllTextLocalizers(this GameObject go)
    {
        foreach (var comp in go.GetComponentsInChildren<TextLocalizer>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }
    }

    internal static void DestroyAllBinders(this GameObject go)
    {
        foreach (var comp in go.GetComponentsInChildren<VisibilityBinder>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }

        foreach (var comp in go.GetComponentsInChildren<StringBinder>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }

        foreach (var comp in go.GetComponentsInChildren<ButtonBinder>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }

        foreach (var comp in go.GetComponentsInChildren<InputBinder>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }
    }
}
