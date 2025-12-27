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

    private Vector3 _cachedControllerPosition;
    private Vector3 _cachedWPos;
    private string[] _cachedTexts;
    private bool _hasSyncPos;
    private Vector3 _cachedSyncPos;
    private Vector3 _cachedSyncWorldPos;

    public void OnGUI()
    {
        if (!InfoDisplay.DebugEnabled) return;
        if (_instance == null) return;
        if (!_instance.IsOnNetwork) return;

        if (_instance is ZombieNetworked zombieNetworked)
        {
            try
            {
                var zombie = zombieNetworked._Zombie;
                if (zombie.mDead) return;

                if (zombieNetworked.ZombieType is ZombieType.Target or ZombieType.Gravestone) return;

                _cachedControllerPosition = zombie.mController.transform.position;
                _cachedWPos = GetWorldPos(_cachedControllerPosition) + new Vector3(85f, 175f, 0f);

                _cachedTexts =
                [
                $"{Enum.GetName(zombieNetworked.ZombieType)} Zombie",
                $"{Enum.GetName(zombie.mZombiePhase)}: {zombie.mPhaseCounter}"
                ];

                DebugRenderHelper.Strings(_cachedWPos.x, _cachedWPos.y + 15f, 1f, 1f, _cachedTexts, Color.white);
                DebugRenderHelper.Box(new(_cachedWPos.x, _cachedWPos.y - 75), new Vector2(100f, 150f), 1f, Color.white);

                _hasSyncPos = zombieNetworked.lastSyncPosX != null;
                if (_hasSyncPos)
                {
                    _cachedSyncWorldPos = new Vector3(
                        GameExtensions.GetBoardXPosFromXPos(zombieNetworked.lastSyncPosX.Value),
                        _cachedControllerPosition.y
                    );
                    _cachedSyncPos = GetWorldPos(_cachedSyncWorldPos) + new Vector3(75f, 125f, 0f);

                    DebugRenderHelper.Line(_cachedWPos, _cachedSyncPos, 1, Color.magenta);
                    DebugRenderHelper.Box(new(_cachedSyncPos.x, _cachedSyncPos.y), new Vector2(50f, 50f), 1f, Color.magenta);
                }
            }
            catch (NullReferenceException)
            {
                if (_cachedTexts != null && _cachedTexts.Length > 0)
                {
                    DebugRenderHelper.Strings(_cachedWPos.x, _cachedWPos.y + 15f, 1f, 1f, _cachedTexts, Color.white);
                    DebugRenderHelper.Box(new(_cachedWPos.x, _cachedWPos.y - 75), new Vector2(100f, 150f), 1f, Color.white);

                    if (_hasSyncPos)
                    {
                        DebugRenderHelper.Line(_cachedWPos, _cachedSyncPos, 1, Color.magenta);
                        DebugRenderHelper.Box(new(_cachedSyncPos.x, _cachedSyncPos.y), new Vector2(50f, 50f), 1f, Color.magenta);
                    }
                }
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