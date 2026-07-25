using System;

namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// A test-focused <see cref="IGameMemory"/> implementation backed by a fixed
    /// byte buffer. Reads and writes are translated through a configurable base
    /// address so in-memory fixtures can mirror PS2 virtual addresses.
    /// </summary>
    public sealed class InMemoryGameMemory : IProcessIdentifiableGameMemory
    {
        public const long DefaultBaseAddress = 0x20000000L;
        public const int DefaultCapacity = 32 * 1024 * 1024; // 32 MB

        private readonly byte[] _buffer;

        public InMemoryGameMemory()
            : this(DefaultBaseAddress, DefaultCapacity)
        {
        }

        public InMemoryGameMemory(long baseAddress, int capacity)
            : this(baseAddress, capacity, 0)
        {
        }

        public InMemoryGameMemory(long baseAddress, int capacity, int processId)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");

            BaseAddress = baseAddress;
            Capacity = capacity;
            ProcessId = processId;
            _buffer = new byte[capacity];
        }

        public long BaseAddress { get; }
        public int Capacity { get; }
        public int ProcessId { get; }

        /// <summary>
        /// Copies the provided data into the buffer starting at <paramref name="offset"/>
        /// relative to the buffer base (not the virtual base address).
        /// </summary>
        public void Load(byte[] data, int offset = 0)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if ((long)offset + data.Length > _buffer.Length)
                throw new ArgumentException("Data does not fit at the specified offset.", nameof(data));

            Buffer.BlockCopy(data, 0, _buffer, offset, data.Length);
        }

        public bool TryRead(long address, byte[] destination, int offset, int count)
        {
            ValidateBufferArgs(destination, offset, count);

            if (count == 0)
                return true;

            if (!TryGetRange(address, count, out int index))
                return false;

            Buffer.BlockCopy(_buffer, index, destination, offset, count);
            return true;
        }

        public bool TryWrite(long address, byte[] source, int offset, int count)
        {
            ValidateBufferArgs(source, offset, count);

            if (count == 0)
                return true;

            if (!TryGetRange(address, count, out int index))
                return false;

            Buffer.BlockCopy(source, offset, _buffer, index, count);
            return true;
        }

        private static void ValidateBufferArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || (long)offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset or count for the supplied buffer.");
        }

        private bool TryGetRange(long address, int count, out int index)
        {
            index = 0;

            try
            {
                checked
                {
                    long relative = address - BaseAddress;
                    if (relative < 0 || relative > _buffer.Length - count)
                        return false;

                    index = (int)relative;
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
