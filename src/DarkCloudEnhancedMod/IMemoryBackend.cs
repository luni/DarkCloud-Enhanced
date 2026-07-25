using System;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Backend used by <see cref="Platform"/> to perform raw memory I/O and
    /// protection changes against a target process or an in-memory snapshot.
    /// </summary>
    internal interface IMemoryBackend
    {
        bool ReadMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesRead);

        bool WriteMemory(IntPtr handle, long address, byte[] buffer, long size, out ulong bytesWritten);

        bool ProtectMemory(IntPtr handle, long address, long size, uint newProtect, out uint oldProtect);
    }
}
