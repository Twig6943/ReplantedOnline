using Il2CppInterop.Runtime.Attributes;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Versus.NetworkSync;

namespace ReplantedOnline.Network.Object.Game;

internal class ZombieNetworked : NetworkClass
{
    internal static Dictionary<Zombie, ZombieNetworked> NetworkedZombies = [];

    internal Zombie _Zombie;
    internal ZombieType ZombieType;
    internal ZombieID ZombieID;
    internal ZombieID RefZombieID;
    internal int GridX;
    internal int GridY;

    public void OnDestroy()
    {
        if (_Zombie != null)
        {
            NetworkedZombies.Remove(_Zombie);
        }
    }

    [HideFromIl2Cpp]
    public override void HandleRpc(SteamNetClient sender, byte rpcId, PacketReader packetReader)
    {
        switch (rpcId)
        {
            case 0:
                HandleSetFollowerZombieIdRpc(packetReader);
                break;
        }
    }

    internal void SendSetFollowerZombieIdRpc(int index, ZombieID zombieID)
    {
        var writer = PacketWriter.Get();
        writer.WriteInt(index);
        writer.WriteInt((int)zombieID);
        this.SendRpc(0, writer, false);
    }

    private void HandleSetFollowerZombieIdRpc(PacketReader packetReader)
    {
        var index = packetReader.ReadInt();
        var zombieId = (ZombieID)packetReader.ReadInt();
        _Zombie?.mFollowerZombieID[index] = zombieId;
    }

    [HideFromIl2Cpp]
    public override void Serialize(PacketWriter packetWriter, bool init)
    {
        if (init)
        {
            packetWriter.WriteInt(GridX);
            packetWriter.WriteInt(GridY);
            packetWriter.WriteInt((int)ZombieID);
            packetWriter.WriteByte((byte)ZombieType);
        }
    }

    [HideFromIl2Cpp]
    public override void Deserialize(PacketReader packetReader, bool init)
    {
        if (init)
        {
            GridX = packetReader.ReadInt();
            GridY = packetReader.ReadInt();
            ZombieID = (ZombieID)packetReader.ReadInt();
            ZombieType = (ZombieType)packetReader.ReadByte();
            _Zombie = SeedPacketSyncPatch.SpawnZombie(ZombieType, GridX, GridY, false);
            _Zombie.DataID = ZombieID;

            NetworkedZombies[_Zombie] = this;
        }
    }
}
