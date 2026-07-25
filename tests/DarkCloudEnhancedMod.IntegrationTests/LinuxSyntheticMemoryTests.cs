using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class LinuxSyntheticMemoryTests
    {
        [SkippableFact]
        public async Task Platform_GetEEMem_FakePcsx2_ResolvesEEmem()
        {
            Skip.IfNot(Platform.IsLinux, "Linux only");

            // Ensure the real process backend is active; snapshot tests may have replaced it.
            Platform.Backend = new ProcessMemoryBackend();

            string fakeExe = BuildFakePcsx2();
            var fake = StartFakePcsx2(fakeExe);
            if (fake == null)
            {
                Assert.Fail("Could not start the synthetic PCSX2 process.");
                return;
            }

            try
            {
                // Wait for the fake process to emit its pid/EEmem line with a timeout.
                Task<string> readLineTask = Task.Run(() => fake.StandardOutput.ReadLine());
                Task completed = await Task.WhenAny(readLineTask, Task.Delay(TimeSpan.FromSeconds(5)));
                if (completed != readLineTask)
                {
                    try { fake.Kill(); } catch { }
                    Assert.Fail("Fake PCSX2 did not emit expected output within the timeout.");
                    return;
                }

                string line = await readLineTask;
                Assert.False(string.IsNullOrEmpty(line), "Fake PCSX2 emitted an empty line.");

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

                long eemem = Platform.GetEEMem(new IntPtr(pid), pid);
                Assert.Equal(expectedEEmem, eemem);

                byte[] buffer = new byte[8];
                Assert.True(Platform.ReadMemory(new IntPtr(pid), eemem, buffer, 8, out ulong bytesRead));
                Assert.Equal(8UL, bytesRead);
                string text = Encoding.ASCII.GetString(buffer);
                Assert.Equal("DarkClou", text);

                long bootAddr = eemem + 0x299540;
                byte[] boot = new byte[4];
                Assert.True(Platform.ReadMemory(new IntPtr(pid), bootAddr, boot, 4, out _));
                Assert.Equal("Dark", Encoding.ASCII.GetString(boot));

                long palFlagAddr = eemem + 0x1F22EA0;
                byte[] flag = new byte[1];
                Assert.True(Platform.ReadMemory(new IntPtr(pid), palFlagAddr, flag, 1, out _));
                Assert.Equal(1, flag[0]);

                byte roundTripValue = 0xAB;
                byte[] writeBuffer = { roundTripValue };
                long writeAddr = eemem + 0x1000;
                Assert.True(Platform.WriteMemory(new IntPtr(pid), writeAddr, writeBuffer, 1, out ulong bytesWritten));
                Assert.Equal(1UL, bytesWritten);

                byte[] readBuffer = new byte[1];
                Assert.True(Platform.ReadMemory(new IntPtr(pid), writeAddr, readBuffer, 1, out _));
                Assert.Equal(roundTripValue, readBuffer[0]);
            }
            finally
            {
                try { fake.Kill(); } catch { }
                fake.WaitForExit(1000);
                fake.Dispose();
            }
        }

        private static string BuildFakePcsx2()
        {
            string sourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "linux_smoke"));
            string fakeC = Path.Combine(sourceDir, "fake_pcsx2.c");
            string fakeExe = Path.Combine(sourceDir, "fake_pcsx2");

            if (!File.Exists(fakeC))
                return null;

            if (!File.Exists(fakeExe) || File.GetLastWriteTimeUtc(fakeC) > File.GetLastWriteTimeUtc(fakeExe))
            {
                using (var gcc = Process.Start(new ProcessStartInfo("gcc", $"-fPIC -fPIE -pie -Wl,-E -o \"{fakeExe}\" \"{fakeC}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }))
                {
                    gcc.WaitForExit();
                    string gccErrors = gcc.StandardError.ReadToEnd();
                    Assert.True(gcc.ExitCode == 0, $"Failed to build fake_pcsx2. GCC output: {gccErrors}");
                }
            }

            return fakeExe;
        }

        private static Process StartFakePcsx2(string fakeExe)
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
