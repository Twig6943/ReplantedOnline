using Il2CppInterop.Runtime.Attributes;
using Il2CppReloaded.Gameplay;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Versus.NetworkSync;

namespace ReplantedOnline.Network.Object.Game;

internal class PlantNetworked : NetworkClass
{
    internal static Dictionary<Plant, PlantNetworked> NetworkedPlants = [];

    internal Plant _Plant;
    internal SeedType SeedType;
    internal SeedType ImitaterType;
    internal int GridX;
    internal int GridY;

    public void OnDestroy()
    {
        if (_Plant != null)
        {
            NetworkedPlants.Remove(_Plant);
        }
    }

    [HideFromIl2Cpp]
    public override void HandleRpc(SteamNetClient sender, byte rpcId, PacketReader packetReader)
    {
    }

    [HideFromIl2Cpp]
    public override void Serialize(PacketWriter packetWriter, bool init)
    {
        if (init)
        {
            packetWriter.WriteInt(GridX);
            packetWriter.WriteInt(GridY);
            packetWriter.WriteByte((byte)SeedType);
            packetWriter.WriteByte((byte)ImitaterType);
        }
    }

    [HideFromIl2Cpp]
    public override void Deserialize(PacketReader packetReader, bool init)
    {
        if (init)
        {
            GridX = packetReader.ReadInt();
            GridY = packetReader.ReadInt();
            SeedType = (SeedType)packetReader.ReadByte();
            ImitaterType = (SeedType)packetReader.ReadByte();
            _Plant = SeedPacketSyncPatch.SpawnPlant(SeedType, ImitaterType, GridX, GridY, false);

            NetworkedPlants[_Plant] = this;
        }
    }
}
