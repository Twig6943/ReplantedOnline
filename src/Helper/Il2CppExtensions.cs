using UnityEngine.Events;

namespace ReplantedOnline.Helper;

internal static class Il2CppExtensions
{
    internal static void AddListener(this UnityEvent unityEvent, Action action)
    {
        unityEvent.AddListener(action);
    }
}
