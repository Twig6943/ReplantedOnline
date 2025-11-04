using Il2CppReloaded.Data;
using UnityEngine;

namespace ReplantedOnline.Modules;

internal static class LevelEntries
{
    private static List<LevelEntryData> AllLevels = [];
    private static Dictionary<string, LevelEntryData> LevelNameLookup = [];
    internal static void Init()
    {
        var levels = Resources.FindObjectsOfTypeAll<LevelEntryData>();
        AllLevels = [.. levels];

        foreach (var level in AllLevels)
        {
            LevelNameLookup[level.FullLevelName] = level;
        }
    }

    internal static LevelEntryData GetLevel(string name)
    {
        if (LevelNameLookup.TryGetValue(name, out var levelData))
        {
            return levelData;
        }
        return default;
    }
}
