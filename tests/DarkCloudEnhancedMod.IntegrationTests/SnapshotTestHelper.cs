using System.Diagnostics;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    internal static class SnapshotTestHelper
    {
        public const long Ps2BaseAddress = 0x20000000L;

        public static byte[] CreateEmptyRam(int size = 32 * 1024 * 1024)
        {
            return new byte[size];
        }

        public static void UseSnapshot(byte[] ram, Region region = Region.NTSC)
        {
            Platform.Backend = new SnapshotMemoryBackend(ram, Ps2BaseAddress);
            Memory.emulatorProcess = Process.GetCurrentProcess();
            Memory.EEMemAddress = Ps2BaseAddress;
            Memory.EEMemOffset = 0L;
            RegionAddresses.RegionDetected = true;
            RegionAddresses.CurrentRegion = region;
        }
    }
}
