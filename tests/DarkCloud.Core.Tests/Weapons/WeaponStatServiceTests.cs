using DarkCloud.Core.Weapons;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Weapons
{
    public class WeaponStatServiceTests
    {
        private const int WeaponOffset = 0x10;

        [Fact]
        public void TryWriteAndReadUShort_WritesExpectedValue()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeWeaponMemoryLayout(0x1020, WeaponOffset);
            var service = new WeaponStatService(memory, layout);

            bool written = service.TryWriteUShort(5, WeaponCharacter.Toan, 0, WeaponStat.Attack, 42);
            Assert.True(written);

            bool read = service.TryReadUShort(5, WeaponCharacter.Toan, 0, WeaponStat.Attack, out ushort value);
            Assert.True(read);
            Assert.Equal(42, value);
        }

        [Fact]
        public void TryWriteUShort_WithCharacterOffset_AppliesOffset()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeWeaponMemoryLayout(0x1020, WeaponOffset);
            var service = new WeaponStatService(memory, layout);

            service.TryWriteUShort(0, WeaponCharacter.Xiao, 0, WeaponStat.Attack, 99);

            bool read = service.TryReadUShort(0, WeaponCharacter.Xiao, 0, WeaponStat.Attack, out ushort value);
            Assert.True(read);
            Assert.Equal(99, value);
        }

        [Fact]
        public void TryWriteByte_WithEffect2Stat_WritesSingleByte()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeWeaponMemoryLayout(0x1020, WeaponOffset);
            var service = new WeaponStatService(memory, layout);

            service.TryWriteByte(1, WeaponCharacter.Toan, 0, WeaponStat.Effect2, 32);

            bool read = service.TryReadByte(1, WeaponCharacter.Toan, 0, WeaponStat.Effect2, out byte value);
            Assert.True(read);
            Assert.Equal(32, value);
        }

        private sealed class FakeWeaponMemoryLayout : IWeaponMemoryLayout
        {
            private readonly long _statBase;

            public FakeWeaponMemoryLayout(long statBase, int weaponOffset)
            {
                _statBase = statBase;
                WeaponOffset = weaponOffset;
            }

            public int WeaponOffset { get; }

            public long LambTransformThresholdAddress => 0;

            public long LambStatsThresholdAddress => 0;

            public long GetStatBaseAddress(WeaponStat stat)
            {
                return _statBase + (long)stat;
            }

            public int GetCharacterOffset(WeaponCharacter character)
            {
                return character == WeaponCharacter.Xiao ? 0x40 : 0;
            }
        }
    }
}
