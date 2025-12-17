using BloomEngine.Menu;
using MelonLoader;

namespace ReplantedOnline.Managers;

/// <summary>
/// Handles initialization and configuration bridging between
/// MelonPreferences and BloomEngine configuration menus.
/// </summary>
internal static class BloomEngineManager
{
    private static MelonPreferences_Category m_configCategory;

    /// <summary>
    /// Ensures MelonPreferences are only initialized once.
    /// </summary>
    private static bool m_hasInit;

    /// <summary>
    /// Initializes MelonPreferences entries and validates stored values.
    /// Safe to call multiple times.
    /// </summary>
    internal static void InitMelon()
    {
        if (m_hasInit) return;
        m_hasInit = true;

        m_configCategory = MelonPreferences.CreateCategory(
            ModInfo.ModName.Replace(" ", ""),
            "configs"
        );
    }

    /// <summary>
    /// Initializes BloomEngine menu integration and registers
    /// the mod's configuration UI.
    /// </summary>
    /// <param name="replantedOnline">The active MelonMod instance.</param>
    internal static void InitBloom(MelonMod replantedOnline)
    {
        InitMelon();
        BloomConfigs.Init();

        var mod = ModMenu.CreateEntry(replantedOnline);
        mod.AddDisplayName(ModInfo.ModName);
        mod.AddDescription("PVZR Online is a mod that adds online support to versus!");
        mod.AddConfig(typeof(BloomConfigs));
        mod.Register();
    }

    /// <summary>
    /// BloomEngine-facing configuration definitions.
    /// </summary>
    internal static class BloomConfigs
    {
        /// <summary>
        /// Initializes BloomEngine config fields and syncs values
        /// with MelonPreferences.
        /// </summary>
        internal static void Init()
        {
        }
    }
}
