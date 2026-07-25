using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class GameAddressTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var address = new GameAddress("Game.BootMarker", 0x20299540L);

            Assert.Equal("Game.BootMarker", address.Name);
            Assert.Equal(0x20299540L, address.NtscAddress);
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
        public void KnownAddresses_BootMarker_IsExpectedValue()
        {
            Assert.Equal(0x20299540L, KnownGameAddresses.BootMarker.NtscAddress);
            Assert.Equal(0x2029BCA0L, KnownGameAddresses.PalBootMarker.NtscAddress);
        }
    }
}
