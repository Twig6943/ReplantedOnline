using ReplantedOnline.Attributes;
using ReplantedOnline.Enums;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Gameplay.Versus.Networked;

namespace ReplantedOnline.Network.RPC.Handlers;

/// <summary>
/// Handles the StartGame RPC for initiating online Versus matches in ReplantedOnline.
/// Responsible for synchronizing game start and seed selection between players.
/// </summary>
[RegisterRPCHandler]
internal sealed class AddLadderHandler : RPCHandler
{
    /// <inheritdoc/>
    internal sealed override RpcType Rpc => RpcType.AddLadder;

    internal static void Send(int theGridX, int theGridY)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteInt(theGridX);
        packetWriter.WriteInt(theGridY);
        NetworkDispatcher.SendRpc(RpcType.AddLadder, packetWriter);
        packetWriter.Recycle();
    }

    /// <inheritdoc/>
    internal sealed override void Handle(SteamNetClient sender, PacketReader packetReader)
    {
        if (sender.Team is PlayerTeam.Plants)
        {
            int gridX = packetReader.ReadInt();
            int gridY = packetReader.ReadInt();
            Instances.GameplayActivity.Board.AddALadderOriginal(gridX, gridY);
        }
    }
}