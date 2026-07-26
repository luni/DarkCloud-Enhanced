using System;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Test-focused <see cref="IMemoryBackend"/> backed by an in-memory byte
    /// array. The supplied <paramref name="address"/> is treated as a PS2
    /// virtual address and mapped relative to <see cref="BaseAddress"/>.
    /// </summary>
    public sealed class SnapshotMemoryBackend : IMemoryBackend
    {
        public const long DefaultBaseAddress = 0x20000000L;

        private readonly byte[] _ram;
        private readonly long _baseAddress;

        public SnapshotMemoryBackend(byte[] ram)
            : this(ram, DefaultBaseAddress)
        {
        }

        public SnapshotMemoryBackend(byte[] ram, long baseAddress)
        {
            _ram = ram ?? throw new ArgumentNullException(nameof(ram));
            _baseAddress = baseAddress;
        }

        public long BaseAddress => _baseAddress;

        public bool ReadMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesRead)
        {
            bytesRead = 0;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            long offset = address - _baseAddress;
            if (offset < 0 || offset + size > _ram.Length || size > buffer.Length)
                return false;

            Buffer.BlockCopy(_ram, (int)offset, buffer, 0, (int)size);
            bytesRead = (ulong)size;
            return true;
        }

        public bool WriteMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesWritten)
        {
            bytesWritten = 0;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            long offset = address - _baseAddress;
            if (offset < 0 || offset + size > _ram.Length || size > buffer.Length)
                return false;

            Buffer.BlockCopy(buffer, 0, _ram, (int)offset, (int)size);
            bytesWritten = (ulong)size;
            return true;
        }

        public bool ProtectMemory(IntPtr handle, long address, long size, uint newProtect, out uint oldProtect)
        {
            // Snapshot memory does not enforce page protection.
            oldProtect = 0;
            return true;
        }

        public void Dispose()
        {
            // The backend does not own the supplied RAM array.
        }
    }
}
