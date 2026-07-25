using System;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Backend used by <see cref="Platform"/> to perform raw memory I/O and
    /// protection changes against a target process or an in-memory snapshot.
    /// Implementations may hold native resources and must be disposed.
    /// </summary>
    internal interface IMemoryBackend : IDisposable
    {
        bool ReadMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesRead);

        bool WriteMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesWritten);

        bool ProtectMemory(IntPtr handle, long address, long size, uint newProtect, out uint oldProtect);
    }
}
