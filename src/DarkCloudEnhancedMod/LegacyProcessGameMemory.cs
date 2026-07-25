using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Adapter that exposes the existing static <see cref="Memory"/> class through
    /// the <see cref="IGameMemory"/> contract. This lets new domain logic depend on
    /// the abstraction while the legacy WinForms application continues to own the
    /// process handle and platform-specific I/O.
    /// </summary>
    internal sealed class LegacyProcessGameMemory : IGameMemory
    {
        public static readonly LegacyProcessGameMemory Instance = new LegacyProcessGameMemory();

        private LegacyProcessGameMemory()
        {
        }

        public bool TryRead(long address, byte[] destination, int offset, int count)
        {
            ValidateBufferArgs(destination, offset, count);

            if (count == 0)
                return Memory.emulatorProcess != null;

            if (Memory.emulatorProcess == null)
                return false;

            byte[] data = Memory.ReadByteArray(address, count);
            if (data == null || data.Length != count)
                return false;

            Buffer.BlockCopy(data, 0, destination, offset, count);
            return true;
        }

        public bool TryWrite(long address, byte[] source, int offset, int count)
        {
            ValidateBufferArgs(source, offset, count);

            if (count == 0)
                return Memory.emulatorProcess != null;

            if (Memory.emulatorProcess == null)
                return false;

            byte[] segment = new byte[count];
            Buffer.BlockCopy(source, offset, segment, 0, count);
            return Memory.Write(address, segment);
        }

        private static void ValidateBufferArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || (long)offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset or count for the supplied buffer.");
        }
    }
}
