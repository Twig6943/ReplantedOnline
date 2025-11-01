using Il2CppSource.Utils;

namespace ReplantedOnline.Modules;

internal class Scenes
{
    internal static void LoadMainMenu() => StateTransitionUtils.Transition("Frontend");

    internal static void LoadVersus()
    {
        StateTransitionUtils.Transition("Gameplay");
        StateTransitionUtils.Transition("Versus");
    }
}
