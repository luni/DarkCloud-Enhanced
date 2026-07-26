using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Handles the Sword of Zeus (weapon ID 296) special effect: when the
    /// weapon levels up its thunder attribute is converted into stored thunder,
    /// which raises the player's max attack.
    /// </summary>
    public sealed class SwordOfZeusService
    {
        public const int SwordOfZeusWeaponId = 296;
        public const byte ElementHudThunder = 2;
        public const byte ElementHudCustom = 5;
        public const ushort MaxStoredThunder = 30000;
        public const ushort BaseMaxAttack = 199;

        private readonly IGameMemory _memory;
        private readonly ISwordOfZeusMemoryLayout _layout;

        public SwordOfZeusService(IGameMemory memory, ISwordOfZeusMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public void ApplyIfSwordOfZeus(int weaponOffset)
        {
            long weaponIdAddress = _layout.GetWeaponIdAddress(weaponOffset);
            if (!TryReadUShort(weaponIdAddress, out ushort weaponId))
                return;

            if (weaponId != SwordOfZeusWeaponId)
                return;

            long thunderAddress = _layout.GetWeaponThunderAddress(weaponOffset);
            if (!TryReadByte(thunderAddress, out byte currentThunder))
                return;

            if (!TryReadUShort(_layout.StoredThunderAddress, out ushort storedThunder))
                storedThunder = 0;

            storedThunder += currentThunder;
            if (storedThunder > MaxStoredThunder)
                storedThunder = MaxStoredThunder;

            TryWriteByte(thunderAddress, 0);

            long elementHudAddress = _layout.GetWeaponElementHudAddress(weaponOffset);
            if (TryReadByte(elementHudAddress, out byte elementHud) && elementHud == ElementHudThunder)
            {
                TryWriteByte(elementHudAddress, ElementHudCustom);
            }

            TryWriteUShort(_layout.StoredThunderAddress, storedThunder);

            ushort maxAttack = CalculateMaxAttack(storedThunder);
            TryWriteUShort(_layout.MaxAttackAddress, maxAttack);
        }

        public static ushort CalculateMaxAttack(ushort storedThunder)
        {
            ushort maxAttack = BaseMaxAttack;

            if (storedThunder > 200)
            {
                if (storedThunder > 500)
                {
                    if (storedThunder > 1000)
                    {
                        if (storedThunder > 2000)
                        {
                            maxAttack = 599;
                            storedThunder -= 2000;
                            maxAttack += (ushort)(storedThunder / 20);
                        }
                        else
                        {
                            maxAttack = 499;
                            storedThunder -= 1000;
                            maxAttack += (ushort)(storedThunder / 10);
                        }
                    }
                    else
                    {
                        maxAttack = 399;
                        storedThunder -= 500;
                        maxAttack += (ushort)(storedThunder / 5);
                    }
                }
                else
                {
                    maxAttack = 299;
                    storedThunder -= 200;
                    maxAttack += (ushort)(storedThunder / 3);
                }
            }
            else
            {
                maxAttack += (ushort)(storedThunder / 2);
            }

            return maxAttack;
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

        private bool TryReadUShort(long address, out ushort value)
        {
            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        private bool TryWriteUShort(long address, ushort value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2);
        }
    }
}
