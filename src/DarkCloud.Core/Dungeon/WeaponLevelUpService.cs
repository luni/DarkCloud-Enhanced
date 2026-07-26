using System;
using System.Collections.Generic;
using DarkCloud.Core.Logging;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Tracks weapon levels while the weapon menu is open and triggers the
    /// Sword of Zeus effect when a weapon levels up.
    /// </summary>
    public sealed class WeaponLevelUpService
    {
        public const int WeaponSlotCount = 10;
        public const byte WeaponMenuMode = 2;
        public const byte PowerPowderMenuMode = 1;
        public const byte PowerPowderMenuValue = 6;

        private readonly IGameMemory _memory;
        private readonly IWeaponLevelUpMemoryLayout _layout;
        private readonly SwordOfZeusService _sozService;
        private readonly IModLogger _logger;

        private readonly byte[] _weaponLevelArray = new byte[WeaponSlotCount];
        private bool _weaponMenuOpen;
        private bool _powerPowderMenuOpen;

        public WeaponLevelUpService(
            IGameMemory memory,
            IWeaponLevelUpMemoryLayout layout,
            SwordOfZeusService sozService,
            IModLogger logger)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _sozService = sozService ?? throw new ArgumentNullException(nameof(sozService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyList<int> Update()
        {
            var leveledUp = new List<int>();

            if (!TryReadByte(_layout.MenuModeAddress, out byte menuMode))
                return leveledUp;

            if (menuMode != WeaponMenuMode && menuMode != PowerPowderMenuMode)
            {
                _weaponMenuOpen = false;
                return leveledUp;
            }

            if (!_weaponMenuOpen)
            {
                SnapshotLevels();
                _weaponMenuOpen = true;
                return leveledUp;
            }

            if (menuMode == PowerPowderMenuMode)
            {
                if (TryReadByte(_layout.PowerPowderMenuAddress, out byte powerPowderMenu) && powerPowderMenu == PowerPowderMenuValue)
                {
                    SnapshotLevels();
                    _powerPowderMenuOpen = true;
                    return leveledUp;
                }

                if (_powerPowderMenuOpen)
                {
                    CheckLevelUps(leveledUp, log: false);
                    _powerPowderMenuOpen = false;
                }
            }
            else if (menuMode == WeaponMenuMode)
            {
                CheckLevelUps(leveledUp, log: true);
            }

            return leveledUp;
        }

        private void SnapshotLevels()
        {
            for (int i = 0; i < WeaponSlotCount; i++)
            {
                if (TryReadByte(_layout.GetWeaponLevelAddress(i), out byte level))
                {
                    _weaponLevelArray[i] = level;
                }
            }
        }

        private void CheckLevelUps(List<int> leveledUp, bool log)
        {
            for (int i = 0; i < WeaponSlotCount; i++)
            {
                if (!TryReadByte(_layout.GetWeaponLevelAddress(i), out byte currentLevel))
                    continue;

                if (currentLevel > _weaponLevelArray[i])
                {
                    if (log)
                    {
                        _logger.Information("Weapon(sword) leveled up!");
                    }

                    _sozService.ApplyIfSwordOfZeus(i);
                    leveledUp.Add(i);
                    _weaponLevelArray[i] = currentLevel;
                }
            }
        }

        private bool TryReadByte(long address, out byte value)
        {
            var buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }
    }
}
