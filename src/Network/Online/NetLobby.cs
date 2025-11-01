using Il2CppSteamworks;
using MelonLoader;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Packet;

namespace ReplantedOnline.Network.Online;

internal class NetLobby
{
    internal static SteamId CurrentLobby = default;
    private static HashSet<SteamId> _connectedMembers = [];

    internal static void Initialize()
    {
        SteamMatchmaking.OnLobbyCreated += (Action<Result, Il2CppSteamworks.Data.Lobby>)((result, data) =>
        {
            OnLobbyCreatedCompleted(result, data);
        });

        SteamMatchmaking.OnLobbyEntered += (Action<Il2CppSteamworks.Data.Lobby>)(data =>
        {
            OnLobbyEnteredCompleted(data);
        });

        SteamMatchmaking.OnLobbyMemberJoined += (Action<Il2CppSteamworks.Data.Lobby, Friend>)((lobby, friend) =>
        {
            OnLobbyMemberJoined(lobby, friend);
        });

        SteamMatchmaking.OnLobbyMemberLeave += (Action<Il2CppSteamworks.Data.Lobby, Friend>)((data, user) =>
        {
            OnLobbyMemberLeave(data, user);
        });

        SteamNetworking.OnP2PSessionRequest += (Action<SteamId>)(steamId =>
        {
            OnP2PSessionRequest(steamId);
        });

        SteamNetworking.OnP2PConnectionFailed += (Action<SteamId, P2PSessionError>)((steamId, error) =>
        {
            OnP2PSessionConnectFail(steamId, error);
        });
    }

    internal static void CreateLobby()
    {
        var createCall = SteamMatchmaking.CreateLobbyAsync(2);
        MelonLogger.Msg("Creating lobby...");
    }

    internal static void JoinLobby(SteamId lobbyId)
    {
        var joinCall = SteamMatchmaking.JoinLobbyAsync(lobbyId);
        MelonLogger.Msg($"Joining lobby: {lobbyId}");
    }

    internal static void LeaveLobby()
    {
        SteamMatchmaking.Internal.LeaveLobby(CurrentLobby);
        SteamNetClient.Clear();
        CurrentLobby = default;
        _connectedMembers.Clear();
        Scenes.LoadMainMenu();
        MelonLogger.Msg("Left lobby");
    }

    internal static string GetLobbyData(string key)
    {
        return SteamMatchmaking.Internal.GetLobbyData(CurrentLobby, key);
    }

    internal static int GetLobbyMemberCount()
    {
        return SteamMatchmaking.Internal.GetNumLobbyMembers(CurrentLobby);
    }

    internal static SteamId GetLobbyMemberByIndex(int index)
    {
        return SteamMatchmaking.Internal.GetLobbyMemberByIndex(CurrentLobby, index);
    }

    internal static SteamId GetLobbyOwner()
    {
        return SteamMatchmaking.Internal.GetLobbyOwner(CurrentLobby);
    }

    private static void OnLobbyCreatedCompleted(Result result, Il2CppSteamworks.Data.Lobby data)
    {
        if (result == Result.OK)
        {
            CurrentLobby = data.Id;
            MelonLogger.Msg($"Lobby created successfully: {CurrentLobby}");

            SteamMatchmaking.Internal.SetLobbyData(CurrentLobby, "mod_version", ModInfo.Version);
            SteamMatchmaking.Internal.SetLobbyData(CurrentLobby, "game", "ReplantedOnline");
            SteamMatchmaking.Internal.SetLobbyType(CurrentLobby, LobbyType.FriendsOnly);

            // Clear connected members when creating a new lobby
            _connectedMembers.Clear();
            _connectedMembers.Add(SteamUser.Internal.GetSteamID());
            AddAllNetClients();
        }
        else
        {
            MelonLogger.Error($"Lobby creation failed");
        }
    }

    private static void OnLobbyEnteredCompleted(Il2CppSteamworks.Data.Lobby data)
    {
        CurrentLobby = data.Id;
        int memberCount = GetLobbyMemberCount();

        Scenes.LoadVersus();
        MelonLogger.Msg($"Joined lobby with {memberCount} players");

        // Clear and rebuild connected members
        _connectedMembers.Clear();
        _connectedMembers.Add(SteamUser.Internal.GetSteamID());
        AddAllNetClients();

        SetupP2PWithLobbyMembers();
    }

    private static void OnLobbyMemberJoined(Il2CppSteamworks.Data.Lobby lobby, Friend user)
    {
        if (lobby.Id != CurrentLobby) return;

        SteamNetClient.Add(user.Id);
        SteamId joinedPlayerId = user.Id;
        MelonLogger.Msg($"Player {joinedPlayerId} joined the lobby");

        // If we're the host, request P2P session with the new player
        if (IsLobbyHost())
        {
            RequestP2PSessionWithPlayer(joinedPlayerId);
        }
    }

    private static void OnLobbyMemberLeave(Il2CppSteamworks.Data.Lobby lobby, Friend user)
    {
        SteamNetClient.Remove(user.Id);
        _connectedMembers.Remove(user.Id);
        MelonLogger.Msg($"Player {user.Id} left the lobby");
    }

    private static void SetupP2PWithLobbyMembers()
    {
        int memberCount = GetLobbyMemberCount();
        for (int i = 0; i < memberCount; i++)
        {
            SteamId member = GetLobbyMemberByIndex(i);
            if (member != SteamUser.Internal.GetSteamID())
            {
                if (IsLobbyHost())
                {
                    // Host initiates P2P connection to all existing members
                    RequestP2PSessionWithPlayer(member);
                }
                _connectedMembers.Add(member);
            }
        }
    }

    private static void RequestP2PSessionWithPlayer(SteamId steamId)
    {
        if (_connectedMembers.Contains(steamId))
        {
            MelonLogger.Msg($"P2P session already established with {steamId}");
            return;
        }

        try
        {
            // Send a small dummy packet to initiate P2P connection
            // This will trigger the remote client's OnP2PSessionRequest
            var packetWriter = PacketWriter.Get();
            packetWriter.AddTag(PacketTag.P2P);
            bool sent = SteamNetworking.SendP2PPacket(steamId, packetWriter.GetBytes(), packetWriter.Length);
            packetWriter.Recycle();

            if (sent)
            {
                MelonLogger.Msg($"Requested P2P session with {steamId}");
            }
            else
            {
                MelonLogger.Warning($"Failed to request P2P session with {steamId}");
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error requesting P2P session with {steamId}: {ex.Message}");
        }
    }

    private static void OnP2PSessionRequest(SteamId steamId)
    {
        if (IsPlayerInOurLobby(steamId))
        {
            SteamNetworking.AcceptP2PSessionWithUser(steamId);
            _connectedMembers.Add(steamId);
            MelonLogger.Msg($"Accepted P2P session with {steamId}");
        }
        else
        {
            MelonLogger.Warning($"Rejected P2P session from non-lobby member: {steamId}");
        }
    }

    private static void OnP2PSessionConnectFail(SteamId steamId, P2PSessionError error)
    {
        MelonLogger.Warning($"P2P session connection failed with {steamId}: {error}");
        _connectedMembers.Remove(steamId);

        if (IsPlayerInOurLobby(steamId) && IsLobbyHost())
        {
            MelonLogger.Msg($"Retrying P2P connection with {steamId}");
        }
    }

    internal static bool IsPlayerInOurLobby(SteamId steamId)
    {
        int memberCount = GetLobbyMemberCount();
        for (int i = 0; i < memberCount; i++)
        {
            if (GetLobbyMemberByIndex(i) == steamId)
                return true;
        }
        return false;
    }

    internal static bool IsInLobby() => CurrentLobby != default;

    internal static bool IsLobbyHost()
    {
        return GetLobbyOwner() == SteamUser.Internal.GetSteamID();
    }

    internal static bool IsLobbyHost(SteamId id)
    {
        return GetLobbyOwner() == id;
    }

    internal static void AddAllNetClients()
    {
        var num = SteamMatchmaking.Internal.GetNumLobbyMembers(CurrentLobby);
        for (int i = 0; i < num; i++)
        {
            var member = SteamMatchmaking.Internal.GetLobbyMemberByIndex(CurrentLobby, i);
            SteamNetClient.Add(member);
        }
    }
}