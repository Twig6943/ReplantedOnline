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

    internal static void DestroyAllTextLocalizers(this MonoBehaviour mono)
    {
        foreach (var comp in mono.GetComponentsInChildren<TextLocalizer>(true))
        {
            UnityEngine.Object.Destroy(comp);
        }
    }
}
