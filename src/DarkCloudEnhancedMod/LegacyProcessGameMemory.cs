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
    internal sealed class LegacyProcessGameMemory : IProcessIdentifiableGameMemory
    {
        private readonly int _processId;

        internal LegacyProcessGameMemory()
        {
            _processId = GetCurrentProcessId();
        }

        public int ProcessId => _processId;

        private static int GetCurrentProcessId()
        {
            var process = Memory.emulatorProcess;
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

        private bool IsCurrentProcess()
        {
            var process = Memory.emulatorProcess;
            if (process == null)
                return false;

            try
            {
                return process.Id == _processId;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }

        public bool TryRead(long address, byte[] destination, int offset, int count)
        {
            ValidateBufferArgs(destination, offset, count);

            if (count == 0)
                return IsCurrentProcess();

            if (!IsCurrentProcess())
                return false;

            if (!Memory.TryReadByteArray(address, count, out byte[] data))
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
