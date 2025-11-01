using Il2CppSteamworks;
using ReplantedOnline.Network;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Helper;

internal static class SteamNetExtensions
{
    /// <summary>
    /// Retrieves a SteamNetClient instance by Steam ID.
    /// </summary>
    /// <param name="steamId">The Steam ID to search for.</param>
    /// <returns>The SteamNetClient instance if found, otherwise null.</returns>
    internal static SteamNetClient GetNetClient(this SteamId steamId)
    {
        if (NetLobby.LobbyData?.AllClients.TryGetValue(steamId, out var client) == true)
        {
            return client;
        }

        return default;
    }

    internal static bool Banned(this SteamId steamId)
    {
        return NetLobby.LobbyData?.Banned.Contains(steamId) == true;
    }

    internal static bool Banned(this SteamNetClient steamNet)
    {
        return NetLobby.LobbyData?.Banned.Contains(steamNet.SteamId) == true;
    }
}
