using Il2CppReloaded.Characters;
using ReplantedOnline.Helper;

namespace ReplantedOnline.Network.Object.Game;

/// <summary>
/// Represents a networked animation controller for synchronizing character animations across the network.
/// </summary>
internal class AnimationControllerNetworked : NetworkClass
{
    internal CharacterAnimationController _AnimationController;

    public void OnDestroy()
    {
        _AnimationController?.RemoveNetworkedLookup();
    }
}
