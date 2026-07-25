using Xunit;

namespace DarkCloudEnhancedMod.IntegrationTests
{
    public class MemoryByteTests
    {
        [Fact]
        public void ReadByte_BootMarker_ReturnsFirstByte()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            int bootOffset = 0x299540;
            ram[bootOffset] = (byte)'D';
            ram[bootOffset + 1] = (byte)'a';
            ram[bootOffset + 2] = (byte)'r';
            ram[bootOffset + 3] = (byte)'k';

            SnapshotTestHelper.UseSnapshot(ram);

            byte bootFirst = Memory.ReadByte(0x20299540L);
            Assert.Equal((byte)'D', bootFirst);
        }

        [Fact]
        public void WriteByte_ReadByte_RoundTrip()
        {
            byte[] ram = SnapshotTestHelper.CreateEmptyRam();
            SnapshotTestHelper.UseSnapshot(ram);

            Memory.WriteByte(0x20001000L, 0xAB);
            byte roundTrip = Memory.ReadByte(0x20001000L);

            Assert.Equal(0xAB, roundTrip);
        }
    }
}
