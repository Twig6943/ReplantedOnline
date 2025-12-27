using Il2CppInterop.Runtime.Attributes;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Network.Object;
using ReplantedOnline.Network.Object.Game;
using UnityEngine;

namespace ReplantedOnline.Monos;

/// <summary>
/// Debugger component for visualizing and debugging networked objects in the game.
/// </summary>
internal sealed class NetworkedDebugger : MonoBehaviour
{
    private NetworkClass _instance;

    /// <summary>
    /// Initializes the debugger with a networked object instance.
    /// </summary>
    /// <param name="networkClass">The networked object instance to debug.</param>
    [HideFromIl2Cpp]
    internal void Initialize(NetworkClass networkClass)
    {
        _instance = networkClass;
    }

    public void OnGUI()
    {
        if (!InfoDisplay.DebugEnabled) return;
        if (_instance == null) return;
        if (!_instance.IsOnNetwork) return;

        if (_instance is ZombieNetworked zombieNetworked)
        {
            if (zombieNetworked.ZombieType is ZombieType.Target or ZombieType.Gravestone) return;

            var wPos = GetWorldPos(zombieNetworked._Zombie.mController.transform.position) + new Vector3(85f, 175f, 0f);
            DebugRenderHelper.Strings(wPos.x, wPos.y + 15f, 1f, 1f,
                [$"{Enum.GetName(zombieNetworked.ZombieType)} Zombie",
                $"{Enum.GetName(zombieNetworked._Zombie.mZombiePhase)}: {zombieNetworked._Zombie.mPhaseCounter}"],
                Color.white);
            DebugRenderHelper.Box(new(wPos.x, wPos.y - 75), new Vector2(100f, 150f), 1f, Color.white);
            if (zombieNetworked.lastSyncPosX != null)
            {
                var syncPos = GetWorldPos(new Vector3(GameExtensions.GetBoardXPosFromXPos(zombieNetworked.lastSyncPosX.Value), zombieNetworked._Zombie.mController.transform.position.y)) + new Vector3(75f, 125f, 0f);
                DebugRenderHelper.Line(wPos, syncPos, 1, Color.magenta);
                DebugRenderHelper.Box(new(syncPos.x, syncPos.y), new Vector2(50f, 50f), 1f, Color.magenta);
            }
        }
    }

    private static Vector3 GetWorldPos(Vector3 worldPos)
    {
        var cam = Camera.main;
        Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

        float distance = Mathf.Round(Vector3.Distance(worldPos, cam.transform.parent.position));
        float size = Mathf.Clamp(1000f / distance, 10f, 50f);
        Vector3 screenPos = new(
            viewportPos.x * Screen.width,
            (1 - viewportPos.y) * Screen.height,
            size
        );

        return screenPos;
    }
}