using Il2CppReloaded.Utils;

namespace ReplantedOnline.Helper;

/// <summary>
/// Provides extension methods for game-specific types to simplify common operations
/// in multiplayer scenarios.
/// </summary>
internal static class GameExtensions
{
    /// <summary>
    /// Gets the local player's item from a multiplayer collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the multiplayer collection.</typeparam>
    /// <param name="multiplayerType">The multiplayer collection instance.</param>
    /// <returns>The item associated with the local player.</returns>
    internal static T LocalItem<T>(this MultiplayerType<T> multiplayerType)
    {
        return multiplayerType[ReplantedOnlineMod.Constants.LOCAL_PLAYER_INDEX];
    }

    /// <summary>
    /// Gets the opponent player's item from a multiplayer collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the multiplayer collection.</typeparam>
    /// <param name="multiplayerType">The multiplayer collection instance.</param>
    /// <returns>The item associated with the opponent player.</returns>
    internal static T OpponentItem<T>(this MultiplayerType<T> multiplayerType)
    {
        return multiplayerType[ReplantedOnlineMod.Constants.OPPONENT_PLAYER_INDEX];
    }
}