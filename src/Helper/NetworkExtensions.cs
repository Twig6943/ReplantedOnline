using Il2CppReloaded.Gameplay;
using ReplantedOnline.Network.Object.Game;

namespace ReplantedOnline.Helper;

/// <summary>
/// Provides extension methods for retrieving network controllers associated with game objects
/// </summary>
internal static class NetworkExtensions
{
    /// <summary>
    /// Gets the network controller associated with a Coin object
    /// </summary>
    /// <param name="coin">The Coin instance to look up</param>
    internal static CoinControllerNetworked GetNetworkedCoinController(this Coin coin)
    {
        // Look up the coin in the global dictionary of networked coin controllers
        if (CoinControllerNetworked.NetworkedCoinControllers.TryGetValue(coin, out var controller))
        {
            return controller;
        }
        return null;
    }

    /// <summary>
    /// Gets the network controller associated with a Zombie object
    /// </summary>
    /// <param name="zombie">The Zombie instance to look up</param>
    internal static ZombieNetworked GetNetworkedZombie(this Zombie zombie)
    {
        // Look up the zombie in the global dictionary of networked zombies
        if (ZombieNetworked.NetworkedZombies.TryGetValue(zombie, out var networkedZombie))
        {
            return networkedZombie;
        }
        return null;
    }

    /// <summary>
    /// Gets the network controller associated with a Plant object
    /// </summary>
    /// <param name="plant">The Plant instance to look up</param>
    internal static PlantNetworked GetNetworkedPlant(this Plant plant)
    {
        // Look up the plant in the global dictionary of networked plants
        if (PlantNetworked.NetworkedPlants.TryGetValue(plant, out var networkedPlant))
        {
            return networkedPlant;
        }
        return null;
    }
}