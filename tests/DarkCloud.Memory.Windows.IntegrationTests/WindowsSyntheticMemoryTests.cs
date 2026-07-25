using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DarkCloudEnhancedMod.Windows.IntegrationTests
{
    public class WindowsSyntheticMemoryTests
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [SkippableFact]
        public async Task Platform_ReadMemory_FakeProcess_ReadsMarkers()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "Windows only");

            Platform.Backend = new ProcessMemoryBackend();

            string fakeExe = BuildFakeProcess();
            var fake = StartFakeProcess(fakeExe);
            if (fake == null)
            {
                Assert.Fail("Could not start the synthetic Windows process.");
                return;
            }

            IntPtr handle = IntPtr.Zero;
            try
            {
                Task<string> readLineTask = Task.Run(() => fake.StandardOutput.ReadLine());
                Task completed = await Task.WhenAny(readLineTask, Task.Delay(TimeSpan.FromSeconds(5)));
                if (completed != readLineTask)
                {
                    try { fake.Kill(); } catch { }
                    Assert.Fail("Fake process did not emit expected output within the timeout.");
                    return;
                }

                string line = await readLineTask;
                Assert.False(string.IsNullOrEmpty(line), "Fake process emitted an empty line.");

                long expectedEEmem = 0;
                int pid = fake.Id;
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    if (p.StartsWith("pid="))
                        int.TryParse(p.Substring(4), out pid);
                    if (p.StartsWith("EEmem=0x"))
                        expectedEEmem = Convert.ToInt64(p.Substring(6), 16);
                }

                Assert.NotEqual(0, expectedEEmem);

                handle = Memory.GetProcessHandle(pid);
                Assert.NotEqual(IntPtr.Zero, handle);

                byte[] buffer = new byte[8];
                Assert.True(Platform.ReadMemory(handle, expectedEEmem, buffer, 8, out ulong bytesRead));
                Assert.Equal(8UL, bytesRead);
                Assert.Equal("DarkClou", Encoding.ASCII.GetString(buffer));

                byte[] boot = new byte[4];
                Assert.True(Platform.ReadMemory(handle, expectedEEmem + 0x299540, boot, 4, out _));
                Assert.Equal("Dark", Encoding.ASCII.GetString(boot));

                byte[] flag = new byte[1];
                Assert.True(Platform.ReadMemory(handle, expectedEEmem + 0x1F22EA0, flag, 1, out _));
                Assert.Equal(1, flag[0]);

                byte roundTripValue = 0xAB;
                byte[] writeBuffer = { roundTripValue };
                long writeAddr = expectedEEmem + 0x1000;
                Assert.True(Platform.WriteMemory(handle, writeAddr, writeBuffer, 1, out ulong bytesWritten));
                Assert.Equal(1UL, bytesWritten);

                byte[] readBuffer = new byte[1];
                Assert.True(Platform.ReadMemory(handle, writeAddr, readBuffer, 1, out _));
                Assert.Equal(roundTripValue, readBuffer[0]);
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    CloseHandle(handle);
                try { fake.Kill(); } catch { }
                fake.WaitForExit(1000);
                fake.Dispose();
            }
        }

        [SkippableFact]
        public void Memory_GetProcess_FindsFakeProcessByName()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "Windows only");

            string fakeExe = BuildFakeProcess();
            var fake = StartFakeProcess(fakeExe);
            if (fake == null)
            {
                Assert.Fail("Could not start the synthetic Windows process.");
                return;
            }

            try
            {
                using (Process found = Memory.GetProcess("FakeProcess"))
                {
                    Assert.NotNull(found);
                    Assert.Equal(fake.Id, found.Id);
                }
            }
            finally
            {
                try { fake.Kill(); } catch { }
                fake.WaitForExit(1000);
                fake.Dispose();
            }
        }

        [SkippableFact]
        public void Platform_ReadMemory_InvalidHandle_ReturnsFalse()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT, "Windows only");

            Platform.Backend = new ProcessMemoryBackend();

            byte[] buffer = new byte[8];
            Assert.False(Platform.ReadMemory(IntPtr.Zero, 0x20000000L, buffer, 8, out ulong bytesRead));
            Assert.Equal(0UL, bytesRead);
        }

        [Fact]
        public void RegionAddresses_Translate_PalRegion_MatchesExpected()
        {
            RegionAddresses.RegionDetected = true;
            RegionAddresses.CurrentRegion = Region.PAL;

            long translated = RegionAddresses.Translate(0x20299540L);
            Assert.Equal(0x2029BCA0L, translated);

            translated = RegionAddresses.Translate(0x21F10020L);
            Assert.Equal(0x21F22EA0L, translated);
        }

        private static string BuildFakeProcess()
        {
            string[] candidates =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FakeProcess.exe"),
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "windows_smoke", "bin", "Release", "FakeProcess.exe")),
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "windows_smoke", "bin", "Debug", "FakeProcess.exe"))
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                    return candidate;
            }

            return null;
        }

        private static Process StartFakeProcess(string fakeExe)
        {
            if (string.IsNullOrEmpty(fakeExe) || !File.Exists(fakeExe))
                return null;

            var start = new ProcessStartInfo(fakeExe)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(fakeExe)
            };

            return Process.Start(start);
        }
    }
}
