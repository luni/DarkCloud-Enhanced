using System.Collections.Generic;
using DarkCloud.Core.Weapons;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Weapons
{
    public class WeaponBalanceServiceTests
    {
        [Fact]
        public void ApplyAll_AppliesAllChanges()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x200);
            var layout = new FakeWeaponMemoryLayout(0x1020, 0x10);
            var statService = new WeaponStatService(memory, layout);
            var changes = new List<IWeaponBalanceChange>
            {
                new WeaponStatBalanceChange(5, WeaponCharacter.Toan, 0, WeaponStat.Attack, 77, WeaponBalanceValueSize.UShort),
                new WeaponStatBalanceChange(5, WeaponCharacter.Toan, 0, WeaponStat.Endurance, 88, WeaponBalanceValueSize.UShort),
            };
            var service = new WeaponBalanceService(statService, changes);

            service.ApplyAll();

            Assert.True(statService.TryReadUShort(5, WeaponCharacter.Toan, 0, WeaponStat.Attack, out ushort attack));
            Assert.Equal(77, attack);
            Assert.True(statService.TryReadUShort(5, WeaponCharacter.Toan, 0, WeaponStat.Endurance, out ushort endurance));
            Assert.Equal(88, endurance);
        }

        [Fact]
        public void ApplyAll_WithAddOperation_AddsToCurrentValue()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x200);
            var layout = new FakeWeaponMemoryLayout(0x1020, 0x10);
            var statService = new WeaponStatService(memory, layout);
            statService.TryWriteUShort(2, WeaponCharacter.Toan, 0, WeaponStat.Attack, 50);

            var changes = new List<IWeaponBalanceChange>
            {
                new WeaponStatBalanceChange(2, WeaponCharacter.Toan, 0, WeaponStat.Attack, 10, WeaponBalanceValueSize.UShort, WeaponBalanceOperation.Add),
            };
            var service = new WeaponBalanceService(statService, changes);
            service.ApplyAll();

            Assert.True(statService.TryReadUShort(2, WeaponCharacter.Toan, 0, WeaponStat.Attack, out ushort value));
            Assert.Equal(60, value);
        }

        [Fact]
        public void WeaponBalanceTable_HasExpectedNumberOfChanges()
        {
            Assert.NotEmpty(WeaponBalanceTable.AllChanges);
            Assert.True(WeaponBalanceTable.AllChanges.Count > 100, $"Expected more than 100 changes, got {WeaponBalanceTable.AllChanges.Count}");
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
