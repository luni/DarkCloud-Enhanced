using System;
using DarkCloud.Memory.Abstractions;
using GameMemory = DarkCloudEnhancedMod.Memory;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Adapter that exposes the existing static <see cref="Memory"/> class through
    /// the <see cref="IGameMemory"/> contract. This lets new domain logic depend on
    /// the abstraction while the legacy WinForms application continues to own the
    /// process handle and platform-specific I/O.
    /// </summary>
    public sealed class LegacyProcessGameMemory : IProcessIdentifiableGameMemory
    {
        public int ProcessId => GetCurrentProcessId();

        private static int GetCurrentProcessId()
        {
            var process = GameMemory.emulatorProcess;
            if (process == null)
                return -1;

            try
            {
                return process.Id;
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
            catch (NotSupportedException)
            {
                return -1;
            }
        }

        private static bool IsCurrentProcess()
        {
            // Verify the cached process object is still present and can report an Id.
            // GetCurrentProcessId handles the null/exception cases and returns -1 when dead.
            return GetCurrentProcessId() != -1;
        }

        public bool TryRead(long address, byte[] destination, int offset, int count)
        {
            ValidateBufferArgs(destination, offset, count);

            if (count == 0)
                return IsCurrentProcess();

            if (!IsCurrentProcess())
                return false;

            if (!GameMemory.TryReadByteArray(address, count, out byte[] data))
                return false;

            Buffer.BlockCopy(data, 0, destination, offset, count);
            return true;
        }

        public bool TryWrite(long address, byte[] source, int offset, int count)
        {
            ValidateBufferArgs(source, offset, count);

            if (count == 0)
                return IsCurrentProcess();

            if (!IsCurrentProcess())
                return false;

            byte[] segment = new byte[count];
            Buffer.BlockCopy(source, offset, segment, 0, count);
            return GameMemory.Write(address, segment);
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
