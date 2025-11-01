using Il2CppSteamworks;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline.Network;

internal class SteamNetClient
{
    internal static List<SteamNetClient> AllClients = [];
    internal readonly SteamId SteamId;
    internal readonly int ClientId;
    internal readonly string Name;
    internal bool IsLocal { get; }
    internal bool IsHost => NetLobby.IsLobbyHost(SteamId);

    internal SteamNetClient(SteamId id)
    {
        SteamId = id;
        ClientId = (int)id.AccountId;
        Name = SteamFriends.Internal.GetFriendPersonaName(SteamId);
        IsLocal = id == SteamUser.Internal.GetSteamID();
    }

    internal static SteamNetClient GetBySteamId(SteamId steamId) => AllClients.FirstOrDefault(c => c.SteamId == steamId);

    internal static void Add(SteamId steamId)
    {
        if (GetBySteamId(steamId) != default) return;

        var client = new SteamNetClient(steamId);
        AllClients.Add(client);
    }

    internal static void Remove(SteamId steamId)
    {
        var client = GetBySteamId(steamId);
        if (client != default)
        {
            AllClients.Remove(client);
        }
    }

    internal static void Clear()
    {
        AllClients.Clear();
    }
}
