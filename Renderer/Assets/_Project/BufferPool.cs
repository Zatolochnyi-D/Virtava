using System.Collections.Generic;

public class PooledBuffer
{
    public bool Available;
    public byte[] Buffer;
}

public static class BufferPool
{
    private static Dictionary<int, List<PooledBuffer>> _pool = new();

    public static byte[] Get(int size)
    {
        if (_pool.TryGetValue(size, out var availableBuffers))
        {
            foreach (var buffer in availableBuffers)
            {
                if (buffer.Available)
                {
                    buffer.Available = false;
                    return buffer.Buffer;
                }
            }
            var newBuffer = new PooledBuffer() { Available = false, Buffer = new byte[size] };
            availableBuffers.Add(newBuffer);
            return newBuffer.Buffer;
        }
        else
        {
            var description = new PooledBuffer() { Available = false, Buffer = new byte[size] };
            _pool[size] = new() { description };
            return description.Buffer;
        }
    }

    public static void Release(byte[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = 0;

        foreach (var description in _pool[buffer.Length])
        {
            if (description.Buffer == buffer)
            {
                description.Available = true;
                break;
            }
        }
    }
}