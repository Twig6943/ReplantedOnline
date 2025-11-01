using Il2CppReloaded.Gameplay;
using Il2CppSteamworks;
using MelonLoader;
using ReplantedOnline.Items.Enums;
using ReplantedOnline.Network.Packet;

namespace ReplantedOnline.Network.Online;

internal class NetworkDispatcher
{
    internal static void Send(PacketWriter packetWriter, bool receiveLocally, PacketTag tag = Items.Enums.PacketTag.None)
    {
        var packet = PacketWriter.Get();
        packet.AddTag(tag);
        packet.WritePacket(packetWriter);

        foreach (var client in SteamNetClient.AllClients)
        {
            if (client.IsLocal && !receiveLocally) continue;

            if (NetLobby.IsPlayerInOurLobby(client.SteamId))
            {
                SteamNetworking.SendP2PPacket(client.SteamId, packet.GetBytes(), packet.Length);
            }
        }

        MelonLogger.Msg($"NetworkDispatcher: Sending Packet -> Size = {packet.Length}");

        packet.Recycle();
    }

    internal static void SendRpc(RpcType rpc, PacketWriter packetWriter, bool receiveLocally = false)
    {
        var packet = PacketWriter.Get();
        packet.WriteByte((byte)rpc);
        packet.WritePacket(packetWriter);

        Send(packet, receiveLocally, PacketTag.Rpc);

        packetWriter.Recycle();
        packet.Recycle();
    }

    internal static void Update()
    {
        while (SteamNetworking.IsP2PPacketAvailable(out uint messageSize))
        {
            var buffer = PacketBuffer.Get();

            buffer.EnsureCapacity(messageSize);

            buffer.Size = messageSize;
            buffer.Steamid = 0;

            if (SteamNetworking.ReadP2PPacket(buffer.Data, ref buffer.Size, ref buffer.Steamid))
            {
                var sender = SteamNetClient.GetBySteamId(buffer.Steamid);
                MelonLogger.Msg($"NetworkDispatcher: Received Packet from {sender.Name} -> Size = {buffer.Size}");

                if (buffer.Size > 0)
                {
                    var receivedData = buffer.ToByteArray();
                    var packetReader = PacketReader.Get(receivedData);
                    Streamline(sender, packetReader);
                }
                else
                {
                    MelonLogger.Error("NetworkDispatcher: Received Packet with zero size");
                }
            }
            else
            {
                MelonLogger.Error("NetworkDispatcher: Failed to read P2P packet");
            }

            buffer.Recycle();
        }
    }

    internal static void Streamline(SteamNetClient sender, PacketReader packetReader)
    {
        var tag = packetReader.GetTag();

        switch (tag)
        {
            case PacketTag.None:
                break;
            case PacketTag.P2P:
                MelonLogger.Msg($"NetworkDispatcher: P2P session established!");
                break;
            case PacketTag.Rpc:
                StreamlineRpc(sender, packetReader);
                break;
        }

        packetReader.Recycle();
    }

    private static void StreamlineRpc(SteamNetClient sender, PacketReader packetReader)
    {
        RpcType rpc = (RpcType)packetReader.ReadByte();
        MelonLogger.Msg($"NetworkDispatcher: Received Rpc from {sender.Name}: {Enum.GetName(rpc)}");

        switch (rpc)
        {
            case RpcType.StartGame:
                var selectionSet = (SelectionSet)packetReader.ReadByte();
                RPC.HandleGameStart(sender, selectionSet);
                break;
            case RpcType.UpdateGameState:
                var state = (GameState)packetReader.ReadByte();
                RPC.HandleUpdateGameState(sender, state);
                break;
        }
    }
}
