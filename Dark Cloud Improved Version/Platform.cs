using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Dark_Cloud_Improved_Version
{
    internal static class Platform
    {
        internal static bool IsLinux { get; } = Environment.OSVersion.Platform == PlatformID.Unix ||
                                                 Environment.OSVersion.Platform == PlatformID.MacOSX ||
                                                 File.Exists("/proc/self/maps");

        // ----- Windows native imports -----

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, out ulong lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, long dwSize, out ulong lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool VirtualProtectEx(IntPtr hProcess, long lpAddress, long dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool DebugActiveProcess(int PID);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool DebugSetProcessKillOnExit(bool boolean);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool DebugActiveProcessStop(int PID);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetLastErrorWin();

        [DllImport("\\Resources\\pcsx2_offsetreader.dll", EntryPoint = "?GetEEMem@@YAJH@Z", CallingConvention = CallingConvention.Cdecl)]
        private static extern long GetEEMemWin(int procID);

        // ----- Linux native imports -----

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern int kill(int pid, int sig);

        internal const int SIGSTOP = 19;
        internal const int SIGCONT = 18;

        private static FileStream _linuxMemStream;
        private static int _linuxPid = -1;
        private static readonly object _linuxLock = new object();

        internal static long GetEEMem(IntPtr processH, int pid)
        {
            if (IsLinux)
                return GetEEMemLinux(pid);

            long variableAddress = GetEEMemWin(pid);
            if (variableAddress == 0)
                return 0;

            byte[] buffer = new byte[8];
            if (ReadProcessMemory(processH, variableAddress, buffer, 8, out ulong _))
                return BitConverter.ToInt64(buffer, 0);

            return 0;
        }

        private static long GetEEMemLinux(int pid)
        {
            long? result = GetEEMemFromElfSymbol(pid);
            if (result.HasValue && result.Value != 0)
                return result.Value;

            return GetEEMemLinuxHeuristic(pid);
        }

        private static long? GetEEMemFromElfSymbol(int pid)
        {
            try
            {
                string exePath = $"/proc/{pid}/exe";
                string cmdline = File.ReadAllText($"/proc/{pid}/cmdline").TrimEnd('\0');
                string exeName = Path.GetFileName(cmdline.Split('\0')[0]);
                if (string.IsNullOrEmpty(exeName))
                    exeName = "pcsx2";

                using (FileStream fs = new FileStream(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // ELF identification
                    byte[] eIdent = br.ReadBytes(16);
                    if (eIdent.Length < 16 ||
                        eIdent[0] != 0x7F || eIdent[1] != (byte)'E' || eIdent[2] != (byte)'L' || eIdent[3] != (byte)'F' ||
                        eIdent[4] != 2) // 64-bit
                        return null;

                    fs.Position = 16;
                    ushort eType = br.ReadUInt16();
                    br.ReadUInt16(); // e_machine
                    br.ReadUInt32(); // e_version
                    br.ReadUInt64(); // e_entry
                    ulong ePhoff = br.ReadUInt64();
                    ulong eShoff = br.ReadUInt64();
                    br.ReadUInt32(); // e_flags
                    br.ReadUInt16(); // e_ehsize
                    br.ReadUInt16(); // e_phentsize
                    ushort ePhnum = br.ReadUInt16();
                    br.ReadUInt16(); // e_shentsize
                    ushort eShnum = br.ReadUInt16();
                    ushort eShstrndx = br.ReadUInt16();

                    if (eShoff == 0 || eShnum == 0)
                        return null;

                    // Read section headers
                    var sections = new List<ElfSection>(eShnum);
                    fs.Position = (long)eShoff;
                    for (int i = 0; i < eShnum; i++)
                    {
                        uint nameOffset = br.ReadUInt32();
                        br.ReadUInt32(); // sh_type
                        br.ReadUInt64(); // sh_flags
                        br.ReadUInt64(); // sh_addr
                        ulong offset = br.ReadUInt64();
                        ulong size = br.ReadUInt64();
                        br.ReadUInt32(); // sh_link
                        br.ReadUInt32(); // sh_info
                        br.ReadUInt64(); // sh_addralign
                        br.ReadUInt64(); // sh_entsize

                        sections.Add(new ElfSection
                        {
                            NameOffset = nameOffset,
                            Offset = offset,
                            Size = size
                        });
                    }

                    // Read section name string table
                    if (eShstrndx >= sections.Count)
                        return null;
                    ElfSection shStrSection = sections[eShstrndx];
                    fs.Position = (long)shStrSection.Offset;
                    byte[] shStrData = br.ReadBytes((int)shStrSection.Size);

                    foreach (ElfSection sec in sections)
                        sec.Name = ReadNullTerminatedAscii(shStrData, (int)sec.NameOffset);

                    ElfSection dynsym = sections.Find(s => s.Name == ".dynsym");
                    ElfSection dynstr = sections.Find(s => s.Name == ".dynstr");
                    if (dynsym == null || dynstr == null)
                        return null;

                    // Read dynamic string table
                    fs.Position = (long)dynstr.Offset;
                    byte[] dynstrData = br.ReadBytes((int)dynstr.Size);

                    // Iterate dynamic symbols looking for "EEmem"
                    fs.Position = (long)dynsym.Offset;
                    int symCount = (int)(dynsym.Size / 24);
                    for (int i = 0; i < symCount; i++)
                    {
                        uint nameOffset = br.ReadUInt32();
                        br.ReadByte();  // st_info
                        br.ReadByte();  // st_other
                        br.ReadUInt16(); // st_shndx
                        ulong stValue = br.ReadUInt64();
                        br.ReadUInt64(); // st_size

                        string symName = ReadNullTerminatedAscii(dynstrData, (int)nameOffset);
                        if (symName != "EEmem")
                            continue;

                        long baseAddr = GetExecutableBaseAddress(pid, exeName);
                        if (baseAddr == 0)
                            return null;

                        // ET_EXEC (2) -> st_value is absolute; ET_DYN (3) -> relative to base
                        long varAddr = eType == 2 ? (long)stValue : baseAddr + (long)stValue;
                        return ReadInt64FromProcMem(pid, varAddr);
                    }
                }
            }
            catch
            {
                // Ignore and fall through to heuristic
            }
            return null;
        }

        private class ElfSection
        {
            internal uint NameOffset;
            internal ulong Offset;
            internal ulong Size;
            internal string Name;
        }

        private static string ReadNullTerminatedAscii(byte[] data, int offset)
        {
            if (offset < 0 || offset >= data.Length)
                return string.Empty;

            int end = offset;
            while (end < data.Length && data[end] != 0)
                end++;

            return Encoding.ASCII.GetString(data, offset, end - offset);
        }

        private static long GetExecutableBaseAddress(int pid, string exeName)
        {
            try
            {
                string maps = File.ReadAllText($"/proc/{pid}/maps");
                long largest = 0;
                foreach (string line in maps.Split('\n'))
                {
                    Match m = Regex.Match(line, @"^([0-9a-fA-F]+)-([0-9a-fA-F]+)\s+(\S+)\s+\S+\s+\S+\s+\S+\s*(.*)$");
                    if (!m.Success)
                        continue;

                    long start = Convert.ToInt64(m.Groups[1].Value, 16);
                    string perms = m.Groups[3].Value;
                    string path = m.Groups[4].Value.Trim();

                    if (perms.StartsWith("r-x", StringComparison.Ordinal) &&
                        Path.GetFileName(path).Equals(exeName, StringComparison.OrdinalIgnoreCase))
                        return start;

                    if (perms.StartsWith("r-x", StringComparison.Ordinal) && start > largest)
                        largest = start;
                }
                return largest;
            }
            catch
            {
                return 0;
            }
        }

        private static long ReadInt64FromProcMem(int pid, long address)
        {
            using (FileStream mem = File.Open($"/proc/{pid}/mem", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                mem.Position = address;
                byte[] buffer = new byte[8];
                int read = mem.Read(buffer, 0, 8);
                if (read != 8)
                    return 0;
                return BitConverter.ToInt64(buffer, 0);
            }
        }

        private static long GetEEMemLinuxHeuristic(int pid)
        {
            try
            {
                string maps = File.ReadAllText($"/proc/{pid}/maps");
                long bestStart = 0, bestSize = 0;
                long pcsx2Start = 0, pcsx2Size = 0;

                foreach (string line in maps.Split('\n'))
                {
                    Match m = Regex.Match(line, @"^([0-9a-fA-F]+)-([0-9a-fA-F]+)\s+(\S+)\s+\S+\s+\S+\s+\S+\s*(.*)$");
                    if (!m.Success)
                        continue;

                    long start = Convert.ToInt64(m.Groups[1].Value, 16);
                    long end = Convert.ToInt64(m.Groups[2].Value, 16);
                    string perms = m.Groups[3].Value;
                    string path = m.Groups[4].Value.Trim();
                    long size = end - start;

                    if (perms != "rw-p" && perms != "rw-s")
                        continue;

                    if (path.IndexOf("pcsx2", StringComparison.OrdinalIgnoreCase) >= 0 && size > pcsx2Size)
                    {
                        pcsx2Start = start;
                        pcsx2Size = size;
                    }

                    if (size > bestSize)
                    {
                        bestStart = start;
                        bestSize = size;
                    }
                }

                if (pcsx2Size >= 0x02000000)
                    return pcsx2Start;

                if (bestSize >= 0x02000000)
                    return bestStart;

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static bool OpenLinuxMemoryStream(int pid)
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

        internal static void CloseLinuxMemoryStream()
        {
            _linuxMemStream?.Dispose();
            _linuxMemStream = null;
            _linuxPid = -1;
        }

        internal static bool ReadMemory(IntPtr processH, long address, byte[] buffer, long size, out ulong bytesRead)
        {
            if (IsLinux)
            {
                lock (_linuxLock)
                {
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

            return ReadProcessMemory(processH, address, buffer, size, out bytesRead);
        }

        internal static bool WriteMemory(IntPtr processH, long address, byte[] buffer, long size, out ulong bytesWritten)
        {
            if (IsLinux)
            {
                lock (_linuxLock)
                {
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

            return WriteProcessMemory(processH, address, buffer, size, out bytesWritten);
        }

        internal static bool ProtectMemory(IntPtr processH, long address, long size, uint newProtect, out uint oldProtect)
        {
            if (IsLinux)
            {
                // /proc/<pid>/mem reads and process_vm_readv do not require explicit protection changes.
                oldProtect = 0;
                return true;
            }

            return VirtualProtectEx(processH, address, size, newProtect, out oldProtect);
        }

        internal static bool SuspendProcess(int pid)
        {
            if (IsLinux)
                return kill(pid, SIGSTOP) == 0;

            if (!DebugActiveProcess(pid))
                return false;

            DebugSetProcessKillOnExit(false);
            return true;
        }

        internal static bool ResumeProcess(int pid)
        {
            if (IsLinux)
                return kill(pid, SIGCONT) == 0;

            return DebugActiveProcessStop(pid);
        }
    }
}
