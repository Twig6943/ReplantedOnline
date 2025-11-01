using Il2CppReloaded.Gameplay;
using Il2CppSource.Utils;
using MelonLoader;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Network.Packet;

namespace ReplantedOnline.Network.Online;

internal static class RPC
{
    internal static void SendStartGame(SelectionSet selectionSet)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteByte((byte)selectionSet);
        NetworkDispatcher.SendRpc(RpcType.StartGame, packetWriter, true);
    }

    internal static void HandleGameStart(SteamNetClient sender, SelectionSet selectionSet)
    {
        if (sender.IsHost)
        {
            MelonLogger.Msg("Game Starting...");
            ReplantAPI.Core.ReplantAPI.GameplayActivity.VersusMode.SelectionSet = selectionSet;
            ReplantAPI.Core.ReplantAPI.GameplayActivity.VersusMode.Phase = VersusPhase.ChoosePlantPacket;
            StateTransitionUtils.Transition("ChooseSeeds");
        }
    }

    internal static void SendUpdateGameState(GameState gameState)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteByte((byte)gameState);
        NetworkDispatcher.SendRpc(RpcType.UpdateGameState, packetWriter, true);
    }

    internal static void HandleUpdateGameState(SteamNetClient sender, GameState gameState)
    {
        if (sender.IsHost)
        {
            MelonLogger.Msg($"Updating GameState: {gameState}");

            switch (gameState)
            {
                case GameState.Lobby:
                    break;
                case GameState.PlantChoosingSeed:
                    ReplantAPI.Core.ReplantAPI.GameplayActivity.VersusMode.Phase = VersusPhase.ChoosePlantPacket;
                    break;
                case GameState.ZombieChoosingSeed:
                    ReplantAPI.Core.ReplantAPI.GameplayActivity.VersusMode.Phase = VersusPhase.ChooseZombiePacket;
                    break;
                case GameState.Gameplay:
                    ReplantAPI.Core.ReplantAPI.GameplayActivity.VersusMode.Phase = VersusPhase.Gameplay;
                    StateTransitionUtils.Transition("Gameplay");
                    break;
            }
        }
    }
}
