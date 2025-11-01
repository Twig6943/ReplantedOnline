using ReplantedOnline.Items.Enums;
using System.Text;

namespace ReplantedOnline.Network.Packet;

internal class PacketReader
{
    private byte[] data = [];
    private int position = 0;
    private static readonly Queue<PacketReader> pool = [];
    private const int MAX_POOL_SIZE = 10;

    internal static PacketReader Get(byte[] data)
    {
        PacketReader reader;

        if (pool.Count > 0)
        {
            reader = pool.Dequeue();
        }
        else
        {
            reader = new PacketReader();
        }

        reader.data = data;
        reader.position = 0;
        return reader;
    }

    internal PacketTag GetTag()
    {
        return (PacketTag)ReadByte();
    }

    internal string ReadString()
    {
        int length = ReadInt();
        if (position + length > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read string");

        string result = Encoding.UTF8.GetString(data, position, length);
        position += length;
        return result;
    }

    internal int ReadInt()
    {
        if (position + 4 > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read int");

        int result = BitConverter.ToInt32(data, position);
        position += 4;
        return result;
    }

    internal uint ReadUInt()
    {
        if (position + 4 > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read uint");

        uint result = BitConverter.ToUInt32(data, position);
        position += 4;
        return result;
    }

    internal float ReadFloat()
    {
        if (position + 4 > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read float");

        float result = BitConverter.ToSingle(data, position);
        position += 4;
        return result;
    }

    internal bool ReadBool()
    {
        if (position >= data.Length)
            throw new IndexOutOfRangeException("Not enough data to read bool");

        return data[position++] == 1;
    }

    internal byte ReadByte()
    {
        if (position >= data.Length)
            throw new IndexOutOfRangeException("Not enough data to read byte");

        return data[position++];
    }

    internal byte[] ReadBytes()
    {
        int length = ReadInt();
        if (position + length > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read bytes");

        byte[] result = new byte[length];
        Array.Copy(data, position, result, 0, length);
        position += length;
        return result;
    }

    internal long ReadLong()
    {
        if (position + 8 > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read long");

        long result = BitConverter.ToInt64(data, position);
        position += 8;
        return result;
    }

    public double ReadDouble()
    {
        if (position + 8 > data.Length)
            throw new IndexOutOfRangeException("Not enough data to read double");

        double result = BitConverter.ToDouble(data, position);
        position += 8;
        return result;
    }

    internal int Remaining => data.Length - position;

    internal void Recycle()
    {
        data = [];
        position = 0;

        if (pool.Count < MAX_POOL_SIZE)
            pool.Enqueue(this);
    }
}
