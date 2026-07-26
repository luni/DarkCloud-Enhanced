using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Default <see cref="IMemoryBackend"/> that reads and writes real process
    /// memory. On Linux it uses /proc/&lt;pid&gt;/mem; on Windows it uses native
    /// process-memory APIs.
    /// </summary>
    internal sealed class ProcessMemoryBackend : IMemoryBackend
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        private FileStream _linuxMemStream;
        private int _linuxPid = -1;
        private bool _disposed;
        private readonly object _linuxLock = new object();

        public bool ReadMemory(IntPtr processH, long address, byte[] buffer, long size, out ulong bytesRead)
        {
            if (Platform.IsLinux)
            {
                lock (_linuxLock)
                {
                    if (_disposed)
                    {
                        bytesRead = 0;
                        return false;
                    }

                    int pid = processH.ToInt32();
                    if (_linuxMemStream == null || _linuxPid != pid)
                    {
                        if (!OpenLinuxMemoryStream(pid))
                        {
                            bytesRead = 0;
                            return false;
                        }
                    }

                    try
                    {
                        _linuxMemStream.Position = address;
                        int toRead = (int)size;
                        int total = 0;
                        while (total < toRead)
                        {
                            int read = _linuxMemStream.Read(buffer, total, toRead - total);
                            if (read == 0)
                                break;
                            total += read;
                        }
                        bytesRead = (ulong)total;
                        return total == toRead;
                    }
                    catch
                    {
                        bytesRead = 0;
                        return false;
                    }
                }
            }

            if (!ReadProcessMemory(processH, new IntPtr(address), buffer, new IntPtr(size), out IntPtr bytesReadPtr))
            {
                bytesRead = 0;
                return false;
            }

            bytesRead = (ulong)bytesReadPtr.ToInt64();
            return true;
        }

        public bool WriteMemory(IntPtr processH, long address, byte[] buffer, long size, out ulong bytesWritten)
        {
            if (Platform.IsLinux)
            {
                lock (_linuxLock)
                {
                    if (_disposed)
                    {
                        bytesWritten = 0;
                        return false;
                    }

                    int pid = processH.ToInt32();
                    if (_linuxMemStream == null || _linuxPid != pid)
                    {
                        if (!OpenLinuxMemoryStream(pid))
                        {
                            bytesWritten = 0;
                            return false;
                        }
                    }

                    try
                    {
                        _linuxMemStream.Position = address;
                        _linuxMemStream.Write(buffer, 0, (int)size);
                        _linuxMemStream.Flush();
                        bytesWritten = (ulong)size;
                        return true;
                    }
                    catch
                    {
                        bytesWritten = 0;
                        return false;
                    }
                }
            }

            if (!WriteProcessMemory(processH, new IntPtr(address), buffer, new IntPtr(size), out IntPtr bytesWrittenPtr))
            {
                bytesWritten = 0;
                return false;
            }

            bytesWritten = (ulong)bytesWrittenPtr.ToInt64();
            return true;
        }

        public bool ProtectMemory(IntPtr processH, long address, long size, uint newProtect, out uint oldProtect)
        {
            if (Platform.IsLinux)
            {
                lock (_linuxLock)
                {
                    if (_disposed)
                    {
                        oldProtect = 0;
                        return false;
                    }
                }

                // /proc/<pid>/mem reads and writes do not require explicit protection changes.
                oldProtect = 0;
                return true;
            }

            return VirtualProtectEx(processH, new IntPtr(address), new IntPtr(size), newProtect, out oldProtect);
        }

        private bool OpenLinuxMemoryStream(int pid)
        {
            CloseLinuxMemoryStream();
            try
            {
                _linuxMemStream = File.Open($"/proc/{pid}/mem", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                _linuxPid = pid;
                return true;
            }
            catch
            {
                _linuxMemStream = null;
                _linuxPid = -1;
                return false;
            }
        }

        private void CloseLinuxMemoryStream()
        {
            try
            {
                _linuxMemStream?.Dispose();
            }
            catch (IOException)
            {
                // The underlying process may have exited before the stream is closed.
            }

            _linuxMemStream = null;
            _linuxPid = -1;
        }

        public void Dispose()
        {
            lock (_linuxLock)
            {
                if (_disposed)
                    return;

                _disposed = true;
                CloseLinuxMemoryStream();
            }
        }
    }
}
