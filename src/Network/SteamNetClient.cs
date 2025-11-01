using Il2CppSteamworks;
using MelonLoader;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Network;

/// <summary>
/// Represents a networked client in ReplantedOnline, managing Steam ID, client information,
/// and network state for players connected via Steamworks P2P.
/// </summary>
internal class SteamNetClient
{
    /// <summary>
    /// Initializes a new instance of the SteamNetClient class.
    /// </summary>
    /// <param name="id">The Steam ID of the client.</param>
    internal SteamNetClient(SteamId id)
    {
        SteamId = id;
        ClientId = (int)id.AccountId;
        Name = SteamFriends.Internal.GetFriendPersonaName(SteamId);
        AmLocal = id == SteamUser.Internal.GetSteamID();
        HasEstablishedP2P = AmLocal;
        MelonLogger.Msg($"[SteamNetClient] P2P connections initialized with {Name} ({SteamId})");
    }

    /// <summary>
    /// The Steam ID of this client.
    /// </summary>
    internal readonly SteamId SteamId;

    /// <summary>
    /// The client ID derived from the Steam account ID.
    /// </summary>
    internal readonly int ClientId;

    /// <summary>
    /// The display name of this client from Steam friends.
    /// </summary>
    internal readonly string Name = "Player";

    /// <summary>
    /// Gets whether this client represents the local player.
    /// </summary>
    internal bool AmLocal { get; }

    /// <summary>
    /// Gets whether this client is the host of the current lobby.
    /// </summary>
    internal bool IsHost => NetLobby.AmLobbyHost(SteamId);

    /// <summary>
    /// Gets whether if P2P has been established with this client
    /// </summary>
    internal bool HasEstablishedP2P { get; set; }

    /// <summary>
    /// Retrieves a SteamNetClient instance by Steam ID.
    /// </summary>
    /// <param name="steamId">The Steam ID to search for.</param>
    /// <returns>The SteamNetClient instance if found, otherwise null.</returns>
    internal static SteamNetClient GetBySteamId(SteamId steamId)
    {
        if (NetLobby.LobbyData.AllClients.TryGetValue(steamId, out var client))
        {
            return client;
        }

        return default;
    }
}