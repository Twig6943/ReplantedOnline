using MelonLoader;
using ReplantedOnline.Items.Attributes;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Managers;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;

namespace ReplantedOnline.Network.RPC.Handlers;

/// <summary>
/// Handles the UpdateGameState RPC for synchronizing game phase transitions in ReplantedOnline.
/// Manages the progression of game states from lobby to gameplay and all intermediate phases.
/// </summary>
/// <remarks>
/// This handler ensures all clients remain in sync with the current game phase,
/// providing a consistent multiplayer experience throughout the match lifecycle.
/// </remarks>
[RegisterRPCHandler]
internal sealed class UpdateGameStateHandler : RPCHandler
{
    /// <inheritdoc/>
    internal sealed override RpcType Rpc => RpcType.UpdateGameState;

    internal static void Send(GameState gameState, bool updateLocally = true)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteByte((byte)gameState);
        NetworkDispatcher.SendRpc(RpcType.UpdateGameState, packetWriter, updateLocally);
    }

    /// <inheritdoc/>
    internal sealed override void Handle(SteamNetClient sender, PacketReader packetReader)
    {
        var gameState = (GameState)packetReader.ReadByte();

        if (!NetLobby.AmLobbyHost())
        {
            NetLobby.LobbyData.LastGameState = gameState;
        }

        // Only process state updates from the actual lobby host
        if (sender.AmHost)
        {
            MelonLogger.Msg($"[RPCHandler] Updating GameState: {gameState}");

            switch (gameState)
            {
                case GameState.Lobby:
                    VersusManager.ResetPlayerInputs();
                    VersusManager.UpdateSideVisuals();
                    break;
                case GameState.HostChoosePlants:
                    VersusManager.UpdatePlayerInputs(!NetLobby.AmLobbyHost());
                    VersusManager.UpdateSideVisuals();
                    break;
                case GameState.HostChooseZombie:
                    VersusManager.UpdatePlayerInputs(NetLobby.AmLobbyHost());
                    VersusManager.UpdateSideVisuals();
                    break;
                default:
                    MelonLogger.Warning($"[RPCHandler] Unknown game state: {gameState}");
                    break;
            }
        }
        else
        {
            MelonLogger.Warning($"[RPCHandler] Rejected GameState update from non-host: {sender.Name}");
        }
    }
}