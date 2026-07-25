using System.Diagnostics;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    internal static class SnapshotTestHelper
    {
        public const long Ps2BaseAddress = 0x20000000L;

        private static Process _currentProcess;

        public static byte[] CreateEmptyRam(int size = 32 * 1024 * 1024)
        {
            return new byte[size];
        }

        public static void UseSnapshot(byte[] ram, Region region = Region.NTSC)
        {
            Platform.Backend = new SnapshotMemoryBackend(ram, Ps2BaseAddress);

            // Reuse a single current-process handle for the test run. We do not
            // dispose it; the Process finalizer will release the handle when the
            // test process exits.
            if (_currentProcess == null)
                _currentProcess = Process.GetCurrentProcess();

            Memory.emulatorProcess = _currentProcess;
            Memory.EEMemAddress = Ps2BaseAddress;
            Memory.EEMemOffset = 0L;
            RegionAddresses.RegionDetected = true;
            RegionAddresses.CurrentRegion = region;
        }
    }
}
