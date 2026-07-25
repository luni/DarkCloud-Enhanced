using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class RegionAddressesTests
    {
        [Fact]
        public void DetectRegion_WithNtscBootMarker_ReportsNtsc()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            int bootOffset = 0x299540;
            ram[bootOffset] = (byte)'D';
            ram[bootOffset + 1] = (byte)'a';
            ram[bootOffset + 2] = (byte)'r';
            ram[bootOffset + 3] = (byte)'k';

            SnapshotTestHelper.UseSnapshot(ram);
            RegionAddresses.RegionDetected = false;
            RegionAddresses.CurrentRegion = Region.NTSC;

            RegionAddresses.DetectRegion();

            Assert.True(RegionAddresses.RegionDetected);
            Assert.Equal(Region.NTSC, RegionAddresses.CurrentRegion);
        }

        [Fact]
        public void Translate_PalRegion_MatchesStandaloneTranslator()
        {
            long ntscAddress = 0x20299540L;
            long[] ntsc = RegionAddresses.NTSC;
            long[] pal = RegionAddresses.PAL;

            var standalone = new RegionAddressTranslator(ntsc, pal);
            long expectedPal = standalone.Translate(GameRegion.Pal, ntscAddress);

            RegionAddresses.RegionDetected = true;
            RegionAddresses.CurrentRegion = Region.PAL;
            long translated = RegionAddresses.Translate(ntscAddress);

            Assert.Equal(expectedPal, translated);
        }
    }
}
