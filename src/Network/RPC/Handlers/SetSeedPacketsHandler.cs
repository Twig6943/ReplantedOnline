using Il2CppReloaded.Gameplay;
using ReplantedOnline.Helper;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;

namespace ReplantedOnline.Network.RPC.Handlers;

internal class SetSeedPacketsHandler : RPCHandler
{
    /// <inheritdoc/>
    internal sealed override RpcType Rpc => RpcType.SetSeedPackets;

    internal static void Send(SeedPacket[] seedPackets)
    {
        var packetWriter = PacketWriter.Get();
        packetWriter.WriteInt(seedPackets.Length);
        foreach (var seedPacket in seedPackets)
        {
            packetWriter.WriteByte((byte)seedPacket.mPacketType);
        }
        NetworkDispatcher.SendRpc(RpcType.SetSeedPackets, packetWriter);
    }

    /// <inheritdoc/>
    internal sealed override void Handle(SteamNetClient sender, PacketReader packetReader)
    {
        var length = packetReader.ReadInt();
        List<SeedType> seedTypes = [];
        for (int i = 0; i < length; i++)
        {
            var seedType = (SeedType)packetReader.ReadByte();
            seedTypes.Add(seedType);
        }

        Utils.SetSeedPackets(1, [.. seedTypes]);
    }
}
