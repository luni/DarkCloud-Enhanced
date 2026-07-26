using System.Collections.Generic;
using DarkCloud.Core.Weapons;
using Xunit;

namespace DarkCloud.Core.Tests.Weapons
{
    public class WeaponSpecialAttributeRollerTests
    {
        [Theory]
        [InlineData(new int[] { 0, 0 }, 32, 0)]     // first roll <50, second roll <50
        [InlineData(new int[] { 0, 50 }, 0, 16)]    // first roll <50, second roll >=50
        [InlineData(new int[] { 50 }, 0, 0)]        // first roll >=50
        public void RollHeavensCloud_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollHeavensCloud();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0, 0 }, 32, 0)]
        [InlineData(new int[] { 0, 50 }, 64, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollDarkCloud_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollDarkCloud();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0, 0 }, 0, 16)]
        [InlineData(new int[] { 0, 50 }, 64, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollBigBang_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollBigBang();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0, 0 }, 0, 8)]
        [InlineData(new int[] { 0, 50 }, 64, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollAtlamilliaSword_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollAtlamilliaSword();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 128, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollDusack_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollDusack();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 0, 8)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollGoddessRing_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollGoddessRing();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 0, 16)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollDestructionRing_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollDestructionRing();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 0, 4)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollSatansRing_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollSatansRing();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 32, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollSkunk_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollSkunk();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        [Theory]
        [InlineData(new int[] { 0 }, 128, 0)]
        [InlineData(new int[] { 50 }, 0, 0)]
        public void RollSwallow_ReturnsExpectedValues(int[] rolls, byte expectedEffect, byte expectedEffect2)
        {
            var roller = CreateRoller(rolls);
            WeaponEffectValues result = roller.RollSwallow();

            Assert.Equal(expectedEffect, result.Effect);
            Assert.Equal(expectedEffect2, result.Effect2);
        }

        private static WeaponSpecialAttributeRoller CreateRoller(int[] rolls)
        {
            var queue = new Queue<int>(rolls);
            return new WeaponSpecialAttributeRoller(() => queue.Dequeue());
        }
    }
}
