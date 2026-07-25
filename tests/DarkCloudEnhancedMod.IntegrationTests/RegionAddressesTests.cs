using DarkCloud.Memory.Abstractions;
using DarkCloud.Memory.Abstractions.Generated;
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
        public void DetectRegion_WithPalBootMarker_ReportsPal()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            // PAL boot marker resolves to the translated address of the NTSC boot marker.
            int bootOffset = 0x29BCA0;
            ram[bootOffset] = (byte)'D';
            ram[bootOffset + 1] = (byte)'a';
            ram[bootOffset + 2] = (byte)'r';
            ram[bootOffset + 3] = (byte)'k';

            SnapshotTestHelper.UseSnapshot(ram);
            RegionAddresses.RegionDetected = false;
            RegionAddresses.CurrentRegion = Region.NTSC;

            RegionAddresses.DetectRegion();

            Assert.True(RegionAddresses.RegionDetected);
            Assert.Equal(Region.PAL, RegionAddresses.CurrentRegion);
        }

        [Fact]
        public void DetectRegion_WithPalFlag_ReportsPal()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            // PAL flag is the translated NTSC region flag.
            int flagOffset = 0x1F22EA0;
            ram[flagOffset] = 1;

            SnapshotTestHelper.UseSnapshot(ram);
            RegionAddresses.RegionDetected = false;
            RegionAddresses.CurrentRegion = Region.NTSC;

            RegionAddresses.DetectRegion();

            Assert.True(RegionAddresses.RegionDetected);
            Assert.Equal(Region.PAL, RegionAddresses.CurrentRegion);
        }

        [Fact]
        public void Generated_PalAddresses_MatchStaticTranslator()
        {
            var translator = new RegionAddressTranslator(RegionAddresses.NTSC, RegionAddresses.PAL);

            foreach (GameAddress address in GameAddresses.All)
            {
                if (address.PalAddress != 0)
                {
                    long translated = translator.Translate(GameRegion.Pal, address.NtscAddress);
                    Assert.Equal(address.PalAddress, translated);
                }
            }
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
