namespace ReplantedOnline.Network.Object;

internal class NetworkIdPool
{
    private readonly Queue<uint> _availableIds = [];

    internal NetworkIdPool(uint start, uint end)
    {
        _start = start;
        _end = end;

        for (uint i = start; i <= end; i++)
        {
            _availableIds.Enqueue(i);
        }
    }

    private uint _start;
    internal uint _end;

    internal uint GetUnusedId()
    {
        if (AvailableCount == 0)
            throw new InvalidOperationException("No available IDs in the pool");

        return _availableIds.Dequeue();
    }

    internal void ReleaseId(uint id)
    {
        if (!_availableIds.Contains(id) && (id >= _start && id <= _end))
        {
            _availableIds.Enqueue(id);
        }
    }

    internal int AvailableCount => _availableIds.Count;
}