using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

// Stand-alone smoke test for the Linux/Flatpak PCSX2 memory path.
// Build with:
//   mcs /out:SmokeTest.exe /r:../../src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe SmokeTest.cs
// Then:
//   ./SmokeTest.exe
class SmokeTest
{
    static int Main(string[] args)
    {
        string fakeExe = "fake_pcsx2";
        var start = new ProcessStartInfo(fakeExe);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;
        start.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        Process fake = null;
        try
        {
            fake = Process.Start(start);
        }
        catch (Exception ex)
        {
            Console.WriteLine("FAIL: could not start {0}: {1}", fakeExe, ex.Message);
            return 1;
        }

        string line = fake.StandardOutput.ReadLine();
        Console.WriteLine("Fake PCSX2 output: {0}", line);

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

        if (expectedEEmem == 0)
        {
            Console.WriteLine("FAIL: could not parse EEmem from fake output");
            fake.Kill();
            return 1;
        }

        Console.WriteLine("Fake pid={0} expected EEmem=0x{1:X}", pid, expectedEEmem);

        string asmPath = args.Length > 0 ? args[0] : "../../src/DarkCloudEnhancedMod/bin/Release/DarkCloudEnhancedMod.exe";
        var asm = Assembly.LoadFrom(asmPath);
        var platform = asm.GetType("DarkCloudEnhancedMod.Platform");
        var getEEMem = platform.GetMethod("GetEEMem", BindingFlags.NonPublic | BindingFlags.Static);
        long eemem = (long)getEEMem.Invoke(null, new object[] { new IntPtr(pid), pid });
        Console.WriteLine("Platform.GetEEMem returned: 0x{0:X}", eemem);

        if (eemem != expectedEEmem)
        {
            Console.WriteLine("FAIL: GetEEMem 0x{0:X} != expected 0x{1:X}", eemem, expectedEEmem);
            fake.Kill();
            return 1;
        }

        var readMem = platform.GetMethod("ReadMemory", BindingFlags.NonPublic | BindingFlags.Static, null,
            new Type[] { typeof(IntPtr), typeof(long), typeof(byte[]), typeof(long), typeof(ulong).MakeByRefType() }, null);
        if (readMem == null)
        {
            Console.WriteLine("FAIL: could not find ReadMemory");
            fake.Kill();
            return 1;
        }

        byte[] buf = new byte[8];
        object[] readArgs = new object[] { new IntPtr(pid), eemem, buf, (long)8, (ulong)0 };
        bool ok = (bool)readMem.Invoke(null, readArgs);
        ulong bytesRead = (ulong)readArgs[4];
        string text = Encoding.ASCII.GetString(buf, 0, (int)bytesRead);
        Console.WriteLine("ReadMemory ok={0} bytesRead={1} data='{2}'", ok, bytesRead, text);

        if (!ok || text != "DarkClou")
        {
            Console.WriteLine("FAIL: could not read DarkClou marker");
            fake.Kill();
            return 1;
        }

        long bootAddr = eemem + 0x299540;
        byte[] boot = new byte[4];
        object[] bootArgs = new object[] { new IntPtr(pid), bootAddr, boot, (long)4, (ulong)0 };
        readMem.Invoke(null, bootArgs);
        string bootStr = Encoding.ASCII.GetString(boot);
        Console.WriteLine("Boot string at 0x20299540: {0}", bootStr);
        if (bootStr != "Dark")
        {
            Console.WriteLine("FAIL: boot string not Dark");
            fake.Kill();
            return 1;
        }

        long palFlagAddr = eemem + 0x1F22EA0;
        byte[] flag = new byte[1];
        object[] flagArgs = new object[] { new IntPtr(pid), palFlagAddr, flag, (long)1, (ulong)0 };
        readMem.Invoke(null, flagArgs);
        Console.WriteLine("PAL flag at 0x21F22EA0: {0}", flag[0]);
        if (flag[0] != 1)
        {
            Console.WriteLine("FAIL: PAL flag not 1");
            fake.Kill();
            return 1;
        }

        // Integration tests against the Memory and RegionAddresses classes.
        var memory = asm.GetType("DarkCloudEnhancedMod.Memory");
        memory.GetField("emulatorProcess", BindingFlags.NonPublic | BindingFlags.Static)
              .SetValue(null, Process.GetProcessById(pid));
        memory.GetField("EEMemAddress", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, eemem);
        memory.GetField("EEMemOffset", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, eemem - 0x20000000L);

        var regionAddresses = asm.GetType("DarkCloudEnhancedMod.RegionAddresses");
        regionAddresses.GetProperty("RegionDetected", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, false, null);
        regionAddresses.GetProperty("CurrentRegion", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 0, null);
        regionAddresses.GetMethod("DetectRegion", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        bool regionDetected = (bool)regionAddresses.GetProperty("RegionDetected", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
        int currentRegion = (int)regionAddresses.GetProperty("CurrentRegion", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null);
        Console.WriteLine("RegionDetected={0} CurrentRegion={1}", regionDetected, currentRegion);
        if (!regionDetected || currentRegion != 0)
        {
            Console.WriteLine("FAIL: region detection did not report NTSC");
            fake.Kill();
            return 1;
        }

        var readByte = memory.GetMethod("ReadByte", BindingFlags.NonPublic | BindingFlags.Static);
        byte bootFirst = (byte)readByte.Invoke(null, new object[] { 0x20299540L });
        Console.WriteLine("Memory.ReadByte boot marker: 0x{0:X} ({1})", bootFirst, (char)bootFirst);
        if (bootFirst != (byte)'D')
        {
            Console.WriteLine("FAIL: Memory.ReadByte did not return 'D'");
            fake.Kill();
            return 1;
        }

        var writeByte = memory.GetMethod("WriteByte", BindingFlags.NonPublic | BindingFlags.Static);
        writeByte.Invoke(null, new object[] { 0x20001000L, (byte)0xAB });
        byte roundTrip = (byte)readByte.Invoke(null, new object[] { 0x20001000L });
        Console.WriteLine("Memory Write/Read byte round-trip: 0x{0:X}", roundTrip);
        if (roundTrip != 0xAB)
        {
            Console.WriteLine("FAIL: Memory Write/Read round-trip failed");
            fake.Kill();
            return 1;
        }

        regionAddresses.GetProperty("CurrentRegion", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, 1, null);
        regionAddresses.GetProperty("RegionDetected", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, true, null);
        var translate = regionAddresses.GetMethod("Translate", BindingFlags.NonPublic | BindingFlags.Static);
        long translated = (long)translate.Invoke(null, new object[] { 0x20299540L });
        long[] ntscArr = (long[])regionAddresses.GetField("NTSC", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        long[] palArr = (long[])regionAddresses.GetField("PAL", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        int idx = Array.IndexOf(ntscArr, 0x20299540L);
        long expectedPal = palArr[idx];
        Console.WriteLine("RegionAddresses.Translate(PAL, 0x20299540) = 0x{0:X}, expected 0x{1:X}", translated, expectedPal);
        if (translated != expectedPal)
        {
            Console.WriteLine("FAIL: PAL address translation mismatch");
            fake.Kill();
            return 1;
        }

        Console.WriteLine("PASS: smoke test and integration checks succeeded");
        fake.Kill();
        return 0;
    }
}
