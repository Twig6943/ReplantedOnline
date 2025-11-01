using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSteamworks;

namespace ReplantedOnline.Network.Packet;

internal class PacketBuffer
{
    public uint Size;
    public SteamId Steamid;
    public Il2CppStructArray<byte> Data = new(1000);
    private static readonly Queue<PacketBuffer> pool = [];
    private const int MAX_POOL_SIZE = 10;

    internal static PacketBuffer Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        return new PacketBuffer();
    }

    internal void EnsureCapacity(uint requiredSize)
    {
        if (Data == null || Data.Length < requiredSize)
        {
            Data = new Il2CppStructArray<byte>((int)requiredSize);
        }
    }

    internal byte[] ToByteArray()
    {
        if (Data == null || Size == 0)
            return [];

        var result = new byte[Size];
        for (int i = 0; i < Size; i++)
        {
            result[i] = Data[i];
        }
        return result;
    }

    internal void Recycle()
    {
        Size = 0;
        Steamid = 0;

        if (pool.Count < MAX_POOL_SIZE)
        {
            pool.Enqueue(this);
        }
        else
        {
            Data = null;
        }
    }
}