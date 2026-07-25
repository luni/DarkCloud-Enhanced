using System;
using System.Text;

namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Default typed writer over an <see cref="IGameMemory"/> backend using
    /// explicit little-endian byte ordering.
    /// </summary>
    public sealed class GameMemoryWriter : IGameMemoryWriter
    {
        private readonly IGameMemory _memory;
        private readonly Encoding _encoding;

        public GameMemoryWriter(IGameMemory memory, Encoding encoding = null)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _encoding = encoding ?? Encoding.ASCII;
        }

        public void WriteByte(long address, byte value)
        {
            if (!_memory.TryWrite(address, new[] { value }, 0, 1))
                ThrowWriteFailed(address, 1);
        }

        public void WriteUInt16(long address, ushort value)
        {
            byte[] buffer =
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF)
            };
            if (!_memory.TryWrite(address, buffer, 0, 2))
                ThrowWriteFailed(address, 2);
        }

        public void WriteUInt32(long address, uint value)
        {
            byte[] buffer =
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF)
            };
            if (!_memory.TryWrite(address, buffer, 0, 4))
                ThrowWriteFailed(address, 4);
        }

        public void WriteInt32(long address, int value)
        {
            WriteUInt32(address, (uint)value);
        }

        public void WriteSingle(long address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            if (!_memory.TryWrite(address, buffer, 0, 4))
                ThrowWriteFailed(address, 4);
        }

        public void WriteString(long address, string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            byte[] buffer = _encoding.GetBytes(value);
            if (buffer.Length == 0)
                return;

            if (!_memory.TryWrite(address, buffer, 0, buffer.Length))
                ThrowWriteFailed(address, buffer.Length);
        }

        private static void ThrowWriteFailed(long address, int count)
        {
            throw new InvalidOperationException(
                $"Could not write {count} byte(s) to address 0x{address:X}.");
        }
    }
}
