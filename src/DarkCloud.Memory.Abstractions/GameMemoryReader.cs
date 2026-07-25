using System;
using System.Text;

namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Default typed reader over an <see cref="IGameMemory"/> backend using
    /// explicit little-endian byte ordering.
    /// </summary>
    public sealed class GameMemoryReader : IGameMemoryReader
    {
        private readonly IGameMemory _memory;
        private readonly Encoding _encoding;

        public GameMemoryReader(IGameMemory memory, Encoding encoding = null)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _encoding = encoding ?? Encoding.ASCII;
        }

        public byte ReadByte(long address)
        {
            byte[] buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
                ThrowReadFailed(address, 1);
            return buffer[0];
        }

        public ushort ReadUInt16(long address)
        {
            byte[] buffer = ReadExact(address, 2);
            return (ushort)(buffer[0] | (buffer[1] << 8));
        }

        public uint ReadUInt32(long address)
        {
            byte[] buffer = ReadExact(address, 4);
            return (uint)(buffer[0]
                | (buffer[1] << 8)
                | (buffer[2] << 16)
                | (buffer[3] << 24));
        }

        public int ReadInt32(long address)
        {
            byte[] buffer = ReadExact(address, 4);
            return buffer[0]
                | (buffer[1] << 8)
                | (buffer[2] << 16)
                | (buffer[3] << 24);
        }

        public float ReadSingle(long address)
        {
            byte[] buffer = ReadExact(address, 4);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public string ReadString(long address, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
            if (length == 0)
                return string.Empty;

            byte[] buffer = ReadExact(address, length);
            return _encoding.GetString(buffer);
        }

        private byte[] ReadExact(long address, int count)
        {
            byte[] buffer = new byte[count];
            if (!_memory.TryRead(address, buffer, 0, count))
                ThrowReadFailed(address, count);
            return buffer;
        }

        private static void ThrowReadFailed(long address, int count)
        {
            throw new InvalidOperationException(
                $"Could not read {count} byte(s) from address 0x{address:X}.");
        }
    }
}
