using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Reads and writes typed weapon-table stats through <see cref="IGameMemory"/>.
    /// </summary>
    public sealed class WeaponStatService
    {
        private readonly IGameMemory _memory;
        private readonly IWeaponMemoryLayout _layout;

        public WeaponStatService(IGameMemory memory, IWeaponMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public long GetStatAddress(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, int statOffset = 0)
        {
            return _layout.GetStatBaseAddress(stat)
                + _layout.GetCharacterOffset(character)
                + ((weaponId - baseWeaponId) * _layout.WeaponOffset)
                + statOffset;
        }

        public bool TryReadByte(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, out byte value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            var buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }

        public bool TryReadUShort(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, out ushort value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        public bool TryReadInt(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, out int value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        public bool TryReadUInt(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, out uint value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToUInt32(buffer, 0);
            return true;
        }

        public bool TryWriteByte(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, byte value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        public bool TryWriteUShort(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, ushort value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2);
        }

        public bool TryWriteInt(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, int value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }

        public bool TryWriteUInt(int weaponId, WeaponCharacter character, int baseWeaponId, WeaponStat stat, uint value, int statOffset = 0)
        {
            long address = GetStatAddress(weaponId, character, baseWeaponId, stat, statOffset);
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }

        public bool TryWriteFloat(long address, float value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }

        public bool TryWriteDouble(long address, double value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 8);
        }
    }
}
