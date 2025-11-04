using Il2CppReloaded.Gameplay;
using ReplantedOnline.Items.Attributes;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Versus.NetworkSync;

namespace ReplantedOnline.Network.RPC.Handlers;

/// <summary>
/// Handles the StartGame RPC for initiating online Versus matches in ReplantedOnline.
/// Responsible for synchronizing game start and seed selection between players.
/// </summary>
[RegisterRPCHandler]
internal sealed class StartMowerHandler : RPCHandler
{
    /// <inheritdoc/>
    internal sealed override RpcType Rpc => RpcType.StartMower;

    internal static void Send(int row)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteInt(row);
        NetworkDispatcher.SendRpc(RpcType.StartMower, packetWriter);
    }

    /// <inheritdoc/>
    internal sealed override void Handle(SteamNetClient sender, PacketReader packetReader)
    {
        var row = packetReader.ReadInt();
        var lawnMower = Instances.GameplayActivity.Board.FindLawnMowerInRow(row);
        if (lawnMower != null && !lawnMower.mDead && lawnMower.mMowerState == LawnMowerState.Ready)
        {
            lawnMower.StartMowerOriginal();
        }
    }
}