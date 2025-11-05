using UnityEngine.Events;

namespace ReplantedOnline.Helper;

/// <summary>
/// Provides extension methods for Il2Cpp types to improve interoperability with C# and simplify common operations.
/// </summary>
internal static class Il2CppExtensions
{
    /// <summary>
    /// Adds a C# Action as a listener to a UnityEvent, simplifying Il2Cpp event subscription.
    /// </summary>
    /// <param name="unityEvent">The UnityEvent to add the listener to.</param>
    /// <param name="action">The Action to be invoked when the event is triggered.</param>
    /// <example>
    /// <code>
    /// button.onClick.AddListener(() => MelonLogger.Msg("Button clicked!"));
    /// </code>
    /// </example>
    internal static void AddListener(this UnityEvent unityEvent, Action action)
    {
        unityEvent.AddListener(action);
    }

    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> managedList)
    {
        var il2cppList = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var item in managedList)
            il2cppList.Add(item);
        return il2cppList;
    }

    public static List<T> ToManagedList<T>(this Il2CppSystem.Collections.Generic.List<T> il2cppList)
    {
        var managedList = new List<T>();
        for (int i = 0; i < il2cppList.Count; i++)
            managedList.Add(il2cppList[i]);
        return managedList;
    }

    public static T[] ToIl2CppArray<T>(this T[] managedArray)
    {
        if (managedArray == null) return null;

        var il2cppArray = new T[managedArray.Length];
        for (int i = 0; i < managedArray.Length; i++)
            il2cppArray[i] = managedArray[i];
        return il2cppArray;
    }

    public static T[] ToManagedArray<T>(this T[] il2cppArray)
    {
        if (il2cppArray == null) return null;

        var managedArray = new T[il2cppArray.Length];
        for (int i = 0; i < il2cppArray.Length; i++)
            managedArray[i] = il2cppArray[i];
        return managedArray;
    }

    public static T[] ToIl2CppArray<T>(this List<T> managedList)
    {
        if (managedList == null) return null;

        var il2cppArray = new T[managedList.Count];
        for (int i = 0; i < managedList.Count; i++)
            il2cppArray[i] = managedList[i];
        return il2cppArray;
    }
    public static T[] ToIl2CppArray<T>(this Il2CppSystem.Collections.Generic.List<T> il2cppList)
    {
        if (il2cppList == null) return null;

        var il2cppArray = new T[il2cppList.Count];
        for (int i = 0; i < il2cppList.Count; i++)
            il2cppArray[i] = il2cppList[i];
        return il2cppArray;
    }

    public static T[] ToIl2CppArray<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null) return null;

        if (enumerable is List<T> list)
            return list.ToIl2CppArray();
        if (enumerable is T[] array)
            return array.ToIl2CppArray();

        var tempList = new List<T>(enumerable);
        return tempList.ToIl2CppArray();
    }
}