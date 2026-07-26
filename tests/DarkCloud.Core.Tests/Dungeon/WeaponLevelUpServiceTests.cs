using System.Collections.Generic;
using DarkCloud.Core.Dungeon;
using DarkCloud.Core.Logging;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class WeaponLevelUpServiceTests
    {
        [Fact]
        public void Update_WhenWeaponMenuOpens_SnapshotsLevels()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x1000);
            var layout = new FakeWeaponLevelUpLayout(0x1000, 0x1001, 0x1010, 0x100);
            var soz = new SwordOfZeusService(memory, new FakeSozLayout());
            var service = new WeaponLevelUpService(memory, layout, soz, NullModLogger.Instance);
            WriteByte(memory, 0x1000, WeaponLevelUpService.WeaponMenuMode);
            WriteByte(memory, 0x1010, 5);

            service.Update();

            // Open again with a higher level -> should report level up
            WriteByte(memory, 0x1010, 7);
            var leveled = service.Update();

            Assert.Single(leveled);
            Assert.Equal(0, leveled[0]);
        }

        [Fact]
        public void Update_WhenNotInMenu_ResetsMenuOpen()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x1000);
            var layout = new FakeWeaponLevelUpLayout(0x1000, 0x1001, 0x1010, 0x100);
            var soz = new SwordOfZeusService(memory, new FakeSozLayout());
            var service = new WeaponLevelUpService(memory, layout, soz, NullModLogger.Instance);
            WriteByte(memory, 0x1000, WeaponLevelUpService.WeaponMenuMode);
            service.Update();

            WriteByte(memory, 0x1000, 0);
            var leveled = service.Update();

            Assert.Empty(leveled);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private sealed class FakeWeaponLevelUpLayout : IWeaponLevelUpMemoryLayout
        {
            private readonly long _menuMode;
            private readonly long _powerPowderMenu;
            private readonly long _weaponLevelBase;
            private readonly int _slotSize;

            public FakeWeaponLevelUpLayout(long menuMode, long powerPowderMenu, long weaponLevelBase, int slotSize)
            {
                _menuMode = menuMode;
                _powerPowderMenu = powerPowderMenu;
                _weaponLevelBase = weaponLevelBase;
                _slotSize = slotSize;
            }

            public long MenuModeAddress => _menuMode;
            public long PowerPowderMenuAddress => _powerPowderMenu;
            public long GetWeaponLevelAddress(int index) => _weaponLevelBase + (index * _slotSize);
            public int WeaponSlotSize => _slotSize;
        }

        private sealed class FakeSozLayout : ISwordOfZeusMemoryLayout
        {
            public long GetWeaponIdAddress(int weaponOffset) => 0;
            public long GetWeaponThunderAddress(int weaponOffset) => 0;
            public long GetWeaponElementHudAddress(int weaponOffset) => 0;
            public long StoredThunderAddress => 0;
            public long MaxAttackAddress => 0;
        }
    }
}
