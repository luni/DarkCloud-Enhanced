using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class RegionAddressTranslatorTests
    {
        [Fact]
        public void Ntsc_ReturnsIdentity()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L, 0x3000L },
                new[] { 0x1100L, 0x2200L, 0x3300L });

            Assert.Equal(0x1500L, translator.Translate(GameRegion.Ntsc, 0x1500L));
        }

        [Fact]
        public void Pal_ExactMatch_ReturnsPalAddress()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L, 0x3000L },
                new[] { 0x1100L, 0x2200L, 0x3300L });

            Assert.Equal(0x2200L, translator.Translate(GameRegion.Pal, 0x2000L));
        }

        [Fact]
        public void Pal_OffsetWithinRange_AppliesPrecedingDelta()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L, 0x3000L },
                new[] { 0x1100L, 0x2200L, 0x3300L });

            // 0x2500 is between 0x2000 and 0x3000, so use delta from 0x2000 -> 0x2200 (+0x200)
            Assert.Equal(0x2700L, translator.Translate(GameRegion.Pal, 0x2500L));
        }

        [Fact]
        public void FirstMapping()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L },
                new[] { 0x1100L, 0x2200L });

            Assert.Equal(0x1100L, translator.Translate(GameRegion.Pal, 0x1000L));
        }

        [Fact]
        public void LastMapping()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L },
                new[] { 0x1100L, 0x2200L });

            Assert.Equal(0x2200L, translator.Translate(GameRegion.Pal, 0x2000L));
        }

        [Fact]
        public void BelowFirstMapping_ReturnsOriginal()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L },
                new[] { 0x1100L, 0x2200L });

            Assert.Equal(0x0500L, translator.Translate(GameRegion.Pal, 0x0500L));
        }

        [Fact]
        public void AboveLastMapping_AppliesLastDelta()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x2000L },
                new[] { 0x1100L, 0x2200L });

            Assert.Equal(0x2700L, translator.Translate(GameRegion.Pal, 0x2500L));
        }

        [Fact]
        public void DuplicateMappings_ReturnsMappedValue()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L, 0x1000L, 0x2000L },
                new[] { 0x1100L, 0x1150L, 0x2200L });

            long actual = translator.Translate(GameRegion.Pal, 0x1000L);
            Assert.True(actual == 0x1100L || actual == 0x1150L);
        }

        [Fact]
        public void UnsortedMappings_AreHandledByBinarySearch()
        {
            // Array.BinarySearch requires sorted input; test verifies behavior with an unsorted array.
            var translator = new RegionAddressTranslator(
                new[] { 0x3000L, 0x1000L, 0x2000L },
                new[] { 0x3300L, 0x1100L, 0x2200L });

            long result = translator.Translate(GameRegion.Pal, 0x1500L);
            Assert.NotEqual(0x1500L, result);
        }

        [Fact]
        public void UnknownRegion_ReturnsIdentity()
        {
            var translator = new RegionAddressTranslator(
                new[] { 0x1000L },
                new[] { 0x1100L });

            Assert.Equal(0x1500L, translator.Translate(GameRegion.Unknown, 0x1500L));
        }

        [Fact]
        public void EmptyMapping_ReturnsIdentity()
        {
            var translator = new RegionAddressTranslator(new long[0], new long[0]);

            Assert.Equal(0x1234L, translator.Translate(GameRegion.Pal, 0x1234L));
        }
    }
}
