using ReplantedOnline.Items.Enums;
using System.Text;

namespace ReplantedOnline.Network.Packet;

internal class PacketWriter
{
    private readonly List<byte> data = [];
    private static readonly Queue<PacketWriter> pool = [];
    private const int MAX_POOL_SIZE = 10;

    internal static PacketWriter Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();

        return new PacketWriter();
    }

    internal void WritePacket(PacketWriter packetWriter)
    {
        data.AddRange(packetWriter.GetBytes());
    }

    internal void WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        WriteInt(bytes.Length);
        data.AddRange(bytes);
    }

    internal void AddTag(PacketTag tag)
    {
        WriteByte((byte)tag);
    }

    internal void WriteInt(int value)
    {
        data.AddRange(BitConverter.GetBytes(value));
    }

    internal void WriteUInt(uint value)
    {
        data.AddRange(BitConverter.GetBytes(value));
    }

    internal void WriteFloat(float value)
    {
        data.AddRange(BitConverter.GetBytes(value));
    }

    internal void WriteBool(bool value)
    {
        data.Add(value ? (byte)1 : (byte)0);
    }

    internal void WriteByte(byte value)
    {
        data.Add(value);
    }

    internal void WriteBytes(byte[] bytes)
    {
        WriteInt(bytes.Length);
        data.AddRange(bytes);
    }

    internal void WriteLong(long value)
    {
        data.AddRange(BitConverter.GetBytes(value));
    }

    internal void WriteDouble(double value)
    {
        data.AddRange(BitConverter.GetBytes(value));
    }

    internal byte[] GetBytes() => [.. data];
    internal int Length => data.Count;

    internal void Recycle()
    {
        data.Clear();

        if (pool.Count < MAX_POOL_SIZE)
            pool.Enqueue(this);
    }
}
