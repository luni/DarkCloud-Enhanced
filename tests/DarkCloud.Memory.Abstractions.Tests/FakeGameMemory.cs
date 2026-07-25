using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Memory.Abstractions.Tests
{
    internal sealed class FakeGameMemory : IGameMemory
    {
        private readonly byte[] _buffer;

        public FakeGameMemory(long baseAddress, int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            BaseAddress = baseAddress;
            Capacity = capacity;
            _buffer = new byte[capacity];
        }

        public long BaseAddress { get; }
        public int Capacity { get; }

        public void Load(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length > _buffer.Length)
                throw new ArgumentException("Data exceeds buffer capacity.", nameof(data));

            Buffer.BlockCopy(data, 0, _buffer, 0, data.Length);
        }

        public bool TryRead(long address, byte[] destination, int offset, int count)
        {
            ValidateBufferArgs(destination, offset, count);

            if (count == 0)
                return true;

            if (!TryGetIndex(address, count, out long index))
                return false;

            Buffer.BlockCopy(_buffer, (int)index, destination, offset, count);
            return true;
        }

        public bool TryWrite(long address, byte[] source, int offset, int count)
        {
            ValidateBufferArgs(source, offset, count);

            if (count == 0)
                return true;

            if (!TryGetIndex(address, count, out long index))
                return false;

            Buffer.BlockCopy(source, offset, _buffer, (int)index, count);
            return true;
        }

        private static void ValidateBufferArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || (long)offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset or count for the supplied buffer.");
        }

        private bool TryGetIndex(long address, int count, out long index)
        {
            index = 0;

            if (count < 1)
                return true;

            try
            {
                checked
                {
                    index = address - BaseAddress;
                    if (index < 0 || index > _buffer.Length - count)
                        return false;
                }
            }
            catch (OverflowException)
            {
                return false;
            }

            return true;
        }
    }
}
