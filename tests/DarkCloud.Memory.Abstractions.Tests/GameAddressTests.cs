using DarkCloud.Memory.Abstractions;
using DarkCloud.Memory.Abstractions.Generated;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class GameAddressTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var address = new GameAddress("Game.BootMarker", 0x20299540L, "UInt32", 0x2029BCA0L);

            Assert.Equal("Game.BootMarker", address.Name);
            Assert.Equal(0x20299540L, address.NtscAddress);
            Assert.Equal("UInt32", address.DataType);
            Assert.Equal(0x2029BCA0L, address.PalAddress);
        }

        [Fact]
        public void Resolve_ForNtsc_ReturnsIdentity()
        {
            var address = new GameAddress("Game.BootMarker", 0x20299540L);
            var translator = new RegionAddressTranslator(
                new[] { 0x20299540L },
                new[] { 0x2029BCA0L });

            Assert.Equal(0x20299540L, address.Resolve(translator, GameRegion.Ntsc));
        }

        [Fact]
        public void Resolve_ForPal_ReturnsTranslatedAddress()
        {
            var address = new GameAddress("Game.BootMarker", 0x20299540L);
            var translator = new RegionAddressTranslator(
                new[] { 0x20299540L },
                new[] { 0x2029BCA0L });

            Assert.Equal(0x2029BCA0L, address.Resolve(translator, GameRegion.Pal));
        }

        [Fact]
        public void Generated_BootMarker_TranslatesToPalBootMarker()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x20299540L },
                new[] { 0x2029BCA0L });

            Assert.Equal(0x20299540L, GameAddresses.Game_BootMarker.Resolve(translator, GameRegion.Ntsc));
            Assert.Equal(0x2029BCA0L, GameAddresses.Game_BootMarker.Resolve(translator, GameRegion.Pal));
        }
    }
}
