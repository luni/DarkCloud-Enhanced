using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Dark_Cloud_Improved_Version
{
    class Memory
    {
        internal static Process emulatorProcess;
        internal static string emulatorName = "pcsx2";
        internal static long EEMemAddress, EEMemOffset;
        internal static long CheckEEMemAddress, CheckEEMemOffset;

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

        [DllImport("\\Resources\\pcsx2_offsetreader.dll", EntryPoint = "?GetEEMem@@YAJH@Z", CallingConvention = CallingConvention.Cdecl)]
        private static extern long GetEEMem(int procID);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer, uint nSize, IntPtr Arguments);

        [DllImport("user32.dll", SetLastError = true)] //Import DLL that will allow us to retrieve processIDs from Window Handles.
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processID); //This is a function within the dll that we are adding to our program.

        [DllImport("kernel32.dll", SetLastError = true)] //Import DLL for reading processes and add the function to our program.
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.ThisCall)]
        public static extern bool VirtualProtect(IntPtr processH, long lpAddress, long lpBuffer, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr processH, long lpAddress, long lpBuffer, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)] //Import for reading process memory.
        private static extern bool ReadProcessMemory(IntPtr processH, long lpBaseAddress, byte[] lpBuffer, long dwSize, out ulong lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)] //Import for writing process memory.
        private static extern bool WriteProcessMemory(IntPtr processH, long lpBaseAddress, byte[] lpBuffer, long dwSize, out ulong lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]  //Import DLL again for Closing Handles to processes and add the function to our program.
        internal static extern bool CloseHandle(IntPtr processH);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DebugActiveProcess(int PID);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DebugSetProcessKillOnExit(bool boolean);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DebugActiveProcessStop(int PID);

        public static void SuspendProcess()
        {
            DebugActiveProcess(emulatorProcess.Id);
            DebugSetProcessKillOnExit(false);
        }

        public static void ResumeProcess() => DebugActiveProcessStop(emulatorProcess.Id);

        internal static string GetSystemMessage(uint errorCode)
        {
            IntPtr messageBuffer = IntPtr.Zero;

            _ = FormatMessage(
                (uint) WinAPIFlags.SystemMessageOptions.All,
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

            if (emulatorProcess != null) {
                CheckEEMemAddress = ReadLong(GetEEMem(emulatorProcess.Id));
                CheckEEMemOffset = CheckEEMemAddress - 0x20000000;

                switch (emulatorProcess.ProcessName) {
                    case "pcsx2":
                        EEMemOffset = 0x00000000;
                        break;
                }

                if (CheckEEMemAddress > 0x0) {
                    EEMemAddress = CheckEEMemAddress;
                    EEMemOffset = CheckEEMemOffset;
                    ModWindow.NightlyVersionCheck();
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
            var found = Process.GetProcesses()
                .Where(p => p.ProcessName
                    .Contains(processToFind))
                .ToList();

            if (found.Count > 1) {
                Console.WriteLine("Found {0} running instances of {1}. Using the last instance found...",
                    found.Count, found[-1].ProcessName);
            }

            return found.LastOrDefault();
        }

        public static IntPtr GetProcessHandle(int processId) => OpenProcess((uint) WinAPIFlags.ProcessModes.All, false, processId);


        internal static byte[] ReadByteArray(long address, long numBytes)  //Read byte array from address + EEMem_Offset
        {
            byte[] dataBuffer = new byte[numBytes];
            ReadProcessMemory(emulatorProcess.Handle, address + EEMemOffset, dataBuffer, dataBuffer.LongLength, out _); //_ seems to act as NULL, we don't need numOfBytesRead
            return dataBuffer;
        }

        internal static byte ReadByte(long address)  //Read byte from address + EEMem_Offset
        {
            var dataBuffer = ReadByteArray(address, 1);
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
            byte[] dataBuffer = new byte[length];
            ReadProcessMemory(emulatorProcess.Handle, address + EEMemOffset, dataBuffer, length, out _);
            return Encoding.GetEncoding(10000).GetString(dataBuffer);
        }

        internal static bool WriteString(long address, string stringToWrite) //Untested
        {
            // http://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
            byte[] dataBuffer = Encoding.GetEncoding(10000).GetBytes(stringToWrite); //Western European (Mac) Encoding Table
            return WriteProcessMemory(emulatorProcess.Handle, address + EEMemOffset, dataBuffer, dataBuffer.LongLength, out _);
        }

        internal static bool Write(long address, byte[] value) => WriteProcessMemory(emulatorProcess.Handle, address + EEMemOffset, value, value.LongLength, out _);

        internal static bool WriteOneByte(long address, byte[] value) => WriteProcessMemory(emulatorProcess.Handle, address + EEMemOffset, value, sizeof(byte), out _);

        internal static bool WriteByte(long address, byte value) => WriteOneByte(address, BitConverter.GetBytes(value));

        internal static void WriteByteArray(long address, byte[] byteArray)  //Write byte array at address + EEMem_Offset
        {
            bool successful = WriteProcessMemory(emulatorProcess.Handle, address + EEMemOffset, byteArray, byteArray.LongLength, out _);

            if (!successful)
                Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + GetLastError() + " - " + GetSystemMessage(GetLastError()));
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

            VirtualProtectEx(emulatorProcess.Handle, startOffset, stopOffset - startOffset, (uint) WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out _); //Change our protection first

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Searching for " + searchString + ". This may take awhile.");

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++) {
                if (ReadString(currentOffset, stringBuffer.LongLength) == searchString) //If we found a match
                    resultsList.Add(currentOffset); //Add it to the list

                ReadString(currentOffset, stringBuffer.LongLength); //Search for our string at the current offset
            }
            return resultsList;
        }

        internal static List<long> IntSearch(long startOffset, long stopOffset, int searchValue)
        {
            List<long> resultsList = new List<long>();

            VirtualProtectEx(emulatorProcess.Handle, startOffset, stopOffset - startOffset, (uint) WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out _); //Change our protection first

            Console.WriteLine(ReusableFunctions.GetDateTimeForLog() + "Searching for " + searchValue + ". This may take awhile.");

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++) {
                if (ReadInt(currentOffset) == searchValue)
                    resultsList.Add(currentOffset);
            }
            return resultsList;
        }

        internal static List<long> ByteArraySearch(long startOffset, long stopOffset, byte[] byteArray)
        {
            List<long> resultsList = new List<long>();

            VirtualProtectEx(emulatorProcess.Handle, startOffset, stopOffset - startOffset, (uint) WinAPIFlags.MemoryPageProtectionModes.ExecuteReadWrite, out _);

            for (long currentOffset = startOffset; currentOffset < stopOffset; currentOffset++) {
                if (ReadByteArray(currentOffset, byteArray.LongLength).SequenceEqual(byteArray)) {
                    resultsList.Add(currentOffset);
                }

                Console.WriteLine("{0:X8}", currentOffset);
            }
            return resultsList;
        }
    }
}
