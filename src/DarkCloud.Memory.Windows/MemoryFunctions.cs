using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DarkCloud.Memory.Windows;

namespace DarkCloudEnhancedMod
{
    class Memory
    {
        internal static Process emulatorProcess;
        internal static string emulatorName = "pcsx2";
        internal static long EEMemAddress, EEMemOffset;
        internal static long CheckEEMemAddress, CheckEEMemOffset;

        internal static IntPtr ProcessHandle
        {
            get
            {
                var process = emulatorProcess;
                if (process == null)
                    return IntPtr.Zero;

                try
                {
                    return Platform.IsLinux ? new IntPtr(process.Id) : process.Handle;
                }
                catch (InvalidOperationException)
                {
                    return IntPtr.Zero;
                }
                catch (NotSupportedException)
                {
                    return IntPtr.Zero;
                }
            }
        }

        internal static class WinAPIFlags
        {
            [Flags]
            internal enum SystemMessageOptions : uint
            {
                FormatMessageAllocateBuffer = 0x0000010,
                FormatMessageIgnoreInserts = 0x0000020,
                FormatMessageFromSystem = 0x00001000,
                All = FormatMessageAllocateBuffer | FormatMessageFromSystem | FormatMessageIgnoreInserts
            }

            [Flags]
            internal enum ProcessModes : ushort
            {
                VMRead = 0x0010,
                VMWrite = 0x0020,
                VMOperation = 0x0008,
                SuspendResume = 0x0800,
                VMReadWrite = VMRead | VMWrite,
                VMOperationSuspendResume = VMOperation | SuspendResume,
                All = VMOperationSuspendResume | VMReadWrite
            }

            [Flags]
            internal enum MemoryPageProtectionModes : byte
            {
                ExecuteReadWrite = 0x40
            }

        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetLastErrorWin();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FormatMessageWin(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr Arguments);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        public static void SuspendProcess() => Platform.SuspendProcess(emulatorProcess.Id);

        public static void ResumeProcess() => Platform.ResumeProcess(emulatorProcess.Id);

        internal static uint GetLastError() => Platform.IsLinux ? 0 : GetLastErrorWin();

        internal static string GetSystemMessage(uint errorCode)
        {
            if (Platform.IsLinux)
                return string.Empty;

            IntPtr messageBuffer = IntPtr.Zero;

            _ = FormatMessageWin(
                (uint)WinAPIFlags.SystemMessageOptions.All,
                IntPtr.Zero,
                errorCode,
                0,
                ref messageBuffer,
                0,
                IntPtr.Zero
            );

            return Marshal.PtrToStringAnsi(messageBuffer);
        }

        public static int Initialize()
        {
            emulatorProcess = GetProcess(emulatorName);

            if (emulatorProcess != null)
            {
                try
                {
                    CheckEEMemAddress = Platform.GetEEMem(ProcessHandle, emulatorProcess.Id);
                    CheckEEMemOffset = CheckEEMemAddress - 0x20000000;

                    switch (emulatorProcess.ProcessName)
                    {
                        case "pcsx2":
                            EEMemOffset = 0x00000000;
                            break;
                    }

                    if (CheckEEMemAddress > 0x0)
                    {
                        EEMemAddress = CheckEEMemAddress;
                        EEMemOffset = CheckEEMemOffset;
                        RegionAddresses.DetectRegion();
                    }
                }
                catch (InvalidOperationException)
                {
                    emulatorProcess = null;
                }
                catch (NotSupportedException)
                {
                    emulatorProcess = null;
                }
            }

            return 0;
        }

        /// <summary>
        /// Function for retrieving the emulator process from running process list
        /// </summary>
        /// <param name="processToFind"></param>
        /// <returns></returns>
        public static Process GetProcess(string processToFind = "pcsx2")
        {
            var candidates = Process.GetProcesses()
                .Where(p => IsPcsx2Process(p, processToFind))
                .ToList();

            if (candidates.Count == 0)
                return null;

            // Prefer the main PCSX2 process (pcsx2-qt or the Flatpak app ID)
            // over helper/wrapper processes.
            var ordered = candidates
                .OrderByDescending(p => Pcsx2ProcessScore(p))
                .ThenBy(p => p.Id)
                .ToList();

            if (ordered.Count > 1)
            {
                Console.WriteLine("Found {0} PCSX2-like processes. Using pid {1} ({2})...",
                    ordered.Count, ordered[0].Id, ordered[0].ProcessName);
            }

            return ordered.FirstOrDefault();
        }

        private static bool IsPcsx2Process(Process p, string processToFind)
        {
            string name = p.ProcessName ?? string.Empty;
            if (name.IndexOf(processToFind, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            // Flatpak/Snap wrappers such as flatpak or bwrap keep the original
            // executable in their command line even when the process name changes.
            if (Platform.IsLinux &&
                !name.Equals("flatpak", StringComparison.OrdinalIgnoreCase) &&
                !name.Equals("bwrap", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string cmdline = File.ReadAllText($"/proc/{p.Id}/cmdline")
                        .Replace('\0', ' ');
                    if (cmdline.IndexOf(processToFind, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch
                {
                    // Ignore processes we can't inspect.
                }
            }

            return false;
        }

        private static int Pcsx2ProcessScore(Process p)
        {
            string name = (p.ProcessName ?? string.Empty).ToLowerInvariant();
            string cmdline = string.Empty;
            if (Platform.IsLinux)
            {
                try
                {
                    cmdline = File.ReadAllText($"/proc/{p.Id}/cmdline")
                        .Replace('\0', ' ')
                        .ToLowerInvariant();
                }
                catch
                {
                    cmdline = string.Empty;
                }
            }
            string all = name + " " + cmdline;
            if (all.Contains("pcsx2-qt") || all.Contains("net.pcsx2.pcsx2"))
                return 2;
            if (all.Contains("pcsx2"))
                return 1;
            return 0;
        }

        public static IntPtr GetProcessHandle(int processId)
        {
            if (Platform.IsLinux)
                return new IntPtr(processId);
            return OpenProcess((uint)WinAPIFlags.ProcessModes.All, false, processId);
        }

        internal const uint PAGE_EXECUTE_READWRITE = (uint)WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite;

        public static bool VirtualProtect(IntPtr processH, long address, long size, uint newProtect, out uint oldProtect)
            => Platform.ProtectMemory(processH, address, size, newProtect, out oldProtect);

        public static bool VirtualProtectEx(IntPtr processH, long address, long size, uint newProtect, out uint oldProtect)
            => Platform.ProtectMemory(processH, address, size, newProtect, out oldProtect);


        internal static byte[] ReadByteArray(long address, long numBytes)  //Read byte array from address + EEMem_Offset
        {
            address = RegionAddresses.Translate(address);
            byte[] dataBuffer = new byte[numBytes];
            Platform.ReadMemory(ProcessHandle, address + EEMemOffset, dataBuffer, dataBuffer.LongLength, out ulong _); //_ seems to act as NULL, we don't need numOfBytesRead
            return dataBuffer;
        }

        internal static byte[] ReadByteArrayRaw(long address, long numBytes)  //Read byte array from address + EEMem_Offset without region translation
        {
            byte[] dataBuffer = new byte[numBytes];
            Platform.ReadMemory(ProcessHandle, address + EEMemOffset, dataBuffer, dataBuffer.LongLength, out ulong _);
            return dataBuffer;
        }

        internal static bool TryReadByteArray(long address, long numBytes, out byte[] data)
        {
            address = RegionAddresses.Translate(address);
            data = new byte[numBytes];
            if (!Platform.ReadMemory(ProcessHandle, address + EEMemOffset, data, data.LongLength, out ulong bytesRead))
                return false;
            return bytesRead == (ulong)numBytes;
        }

        internal static byte ReadByte(long address)  //Read byte from address + EEMem_Offset
        {
            var dataBuffer = ReadByteArray(address, 1);
            return dataBuffer[0];
        }

        internal static byte ReadByteRaw(long address)  //Read byte from address + EEMem_Offset without region translation
        {
            var dataBuffer = ReadByteArrayRaw(address, 1);
            return dataBuffer[0];
        }

        internal static ushort ReadUShort(long address)  //Read unsigned short from address + EEMem_Offset
        {
            var dataBuffer = ReadByteArray(address, 2);
            return BitConverter.ToUInt16(dataBuffer, 0);
        }

        internal static short ReadShort(long address)
        {
            var dataBuffer = ReadByteArray(address, 2);
            return BitConverter.ToInt16(dataBuffer, 0);
        }

        internal static uint ReadUInt(long address)
        {
            var dataBuffer = ReadByteArray(address, 4);
            return BitConverter.ToUInt32(dataBuffer, 0);
        }

        internal static int ReadInt(long address)
        {
            var dataBuffer = ReadByteArray(address, 4);
            return BitConverter.ToInt32(dataBuffer, 0);
        }

        internal static int ReadIntRaw(long address)
        {
            var dataBuffer = ReadByteArrayRaw(address, 4);
            return BitConverter.ToInt32(dataBuffer, 0);
        }

        internal static float ReadFloat(long address)
        {
            var dataBuffer = ReadByteArray(address, 4);
            return BitConverter.ToSingle(dataBuffer, 0);
        }

        internal static double ReadDouble(long address)
        {
            var dataBuffer = ReadByteArray(address, 8);
            return BitConverter.ToDouble(dataBuffer, 0);
        }

        internal static long ReadLong(long address)
        {
            var dataBuffer = ReadByteArray(address, 8);
            return BitConverter.ToInt64(dataBuffer, 0);
        }

        internal static string ReadString(long address, long length)
        {
            // http://stackoverflow.com/questions/1003275/how-to-convert-byte-to-string
            address = RegionAddresses.Translate(address);
            byte[] dataBuffer = new byte[length];
            Platform.ReadMemory(ProcessHandle, address + EEMemOffset, dataBuffer, length, out ulong _);
            return Encoding.GetEncoding(10000).GetString(dataBuffer);
        }

        internal static bool WriteString(long address, string stringToWrite) //Untested
        {
            // http://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
            address = RegionAddresses.Translate(address);
            byte[] dataBuffer = Encoding.GetEncoding(10000).GetBytes(stringToWrite); //Western European (Mac) Encoding Table
            return Platform.WriteMemory(ProcessHandle, address + EEMemOffset, dataBuffer, dataBuffer.LongLength, out ulong _);
        }

        internal static bool Write(long address, byte[] value)
        {
            address = RegionAddresses.Translate(address);
            return Platform.WriteMemory(ProcessHandle, address + EEMemOffset, value, value.LongLength, out ulong _);
        }

        internal static bool WriteOneByte(long address, byte[] value)
        {
            address = RegionAddresses.Translate(address);
            return Platform.WriteMemory(ProcessHandle, address + EEMemOffset, value, sizeof(byte), out ulong _);
        }

        internal static bool WriteByte(long address, byte value) => WriteOneByte(address, BitConverter.GetBytes(value));

        internal static void WriteByteArray(long address, byte[] byteArray)  //Write byte array at address + EEMem_Offset
        {
            address = RegionAddresses.Translate(address);
            bool successful = Platform.WriteMemory(ProcessHandle, address + EEMemOffset, byteArray, byteArray.LongLength, out ulong _);

            if (!successful)
                Console.WriteLine(LogTimestamp.Now() + GetLastError() + " - " + GetSystemMessage(GetLastError()));
        }

        internal static bool WriteUShort(long address, ushort value) => Write(address, BitConverter.GetBytes(value));

        internal static bool WriteInt(long address, int value) => Write(address, BitConverter.GetBytes(value));

        internal static bool WriteUInt(long address, uint value) => Write(address, BitConverter.GetBytes(value));

        internal static bool WriteFloat(long address, float value) => Write(address, BitConverter.GetBytes(value));

        internal static bool WriteDouble(long address, double value) => Write(address, BitConverter.GetBytes(value));

        internal static List<long> StringSearch(long startOffset, long stopOffset, string searchString)
        {
            byte[] stringBuffer = new byte[searchString.LongCount()];
            List<long> resultsList = new List<long>();

            Platform.ProtectMemory(ProcessHandle, startOffset, stopOffset - startOffset, (uint)WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out uint _); //Change our protection first

            Console.WriteLine(LogTimestamp.Now() + "Searching for " + searchString + ". This may take awhile.");

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++)
            {
                if (ReadString(currentOffset, stringBuffer.LongLength) == searchString) //If we found a match
                    resultsList.Add(currentOffset); //Add it to the list

                ReadString(currentOffset, stringBuffer.LongLength); //Search for our string at the current offset
            }
            return resultsList;
        }

        internal static List<long> IntSearch(long startOffset, long stopOffset, int searchValue)
        {
            List<long> resultsList = new List<long>();

            Platform.ProtectMemory(ProcessHandle, startOffset, stopOffset - startOffset, (uint)WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out uint _); //Change our protection first

            Console.WriteLine(LogTimestamp.Now() + "Searching for " + searchValue + ". This may take awhile.");

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++)
            {
                if (ReadInt(currentOffset) == searchValue)
                    resultsList.Add(currentOffset);
            }
            return resultsList;
        }

        internal static List<long> ByteArraySearch(long startOffset, long stopOffset, byte[] byteArray)
        {
            List<long> resultsList = new List<long>();

            Platform.ProtectMemory(ProcessHandle, startOffset, stopOffset - startOffset, (uint)WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out uint _);

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++)
            {
                if (ReadByteArray(currentOffset, byteArray.LongLength).SequenceEqual(byteArray))
                {
                    resultsList.Add(currentOffset);
                }

                Console.WriteLine("{0:X8}", currentOffset);
            }
            return resultsList;
        }
    }
}
