using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Applies and reverts the empty synth-sphere "boost to +5" rule for a
    /// weapon slot. The service performs the memory I/O through the supplied
    /// <see cref="IGameMemory"/> so the rule can be exercised against an
    /// in-memory buffer in tests.
    /// </summary>
    public sealed class WeaponSynthSphereService
    {
        private readonly IGameMemory _memory;

        public const int SynthSphereItemId = 177; // Items.synthsphere
        public const int TargetLevel = 5;

        public WeaponSynthSphereService(IGameMemory memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        }

        public bool TryApplyBoost(WeaponSlotAddresses addresses)
        {
            if (!TryReadUShort(addresses.Slot1ItemId, out ushort slot1ItemId))
                return false;
            if (!TryReadUShort(addresses.Slot1SynthesisedItemId, out ushort slot1SynthesisedItemId))
                return false;

            if (slot1ItemId != SynthSphereItemId || slot1SynthesisedItemId != 0)
                return TryRevertIfNeeded(addresses);

            if (!TryReadByte(addresses.Level, out byte level))
                return false;
            if (!TryReadUShort(addresses.Attack, out ushort attack))
                return false;
            if (!TryReadUShort(addresses.Endurance, out ushort endurance))
                return false;
            if (!TryReadUShort(addresses.Speed, out ushort speed))
                return false;
            if (!TryReadUShort(addresses.Magic, out ushort magic))
                return false;
            if (!TryReadUShort(addresses.HasChangedBySynth, out ushort hasChangedBySynth))
                return false;

            int diffLevel = TargetLevel - level;
            if (diffLevel > 0 && hasChangedBySynth == 0)
            {
                TryWriteByte(addresses.Level, (byte)TargetLevel);
                TryWriteUShort(addresses.Attack, (ushort)(attack + diffLevel));
                TryWriteUShort(addresses.Endurance, (ushort)(endurance + diffLevel));
                TryWriteUShort(addresses.Speed, (ushort)(speed + diffLevel));
                TryWriteUShort(addresses.Magic, (ushort)(magic + diffLevel));
                TryWriteUShort(addresses.WeaponFormerStatsValue, (ushort)diffLevel);
                TryWriteUShort(addresses.HasChangedBySynth, 1);
                return true;
            }

            return false;
        }

        private bool TryRevertIfNeeded(WeaponSlotAddresses addresses)
        {
            if (!TryReadByte(addresses.Level, out byte level))
                return false;
            if (!TryReadUShort(addresses.HasChangedBySynth, out ushort hasChangedBySynth))
                return false;

            int diffLevel = TargetLevel - level;
            if (diffLevel == 0 && hasChangedBySynth == 1)
            {
                if (!TryReadUShort(addresses.WeaponFormerStatsValue, out ushort diffLevelBeforeChange))
                    return false;
                if (!TryReadUShort(addresses.Attack, out ushort attack))
                    return false;
                if (!TryReadUShort(addresses.Endurance, out ushort endurance))
                    return false;
                if (!TryReadUShort(addresses.Speed, out ushort speed))
                    return false;
                if (!TryReadUShort(addresses.Magic, out ushort magic))
                    return false;

                TryWriteUShort(addresses.Level, (ushort)(TargetLevel - diffLevelBeforeChange));
                TryWriteUShort(addresses.Attack, (ushort)(attack - diffLevelBeforeChange));
                TryWriteUShort(addresses.Endurance, (ushort)(endurance - diffLevelBeforeChange));
                TryWriteUShort(addresses.Speed, (ushort)(speed - diffLevelBeforeChange));
                TryWriteUShort(addresses.Magic, (ushort)(magic - diffLevelBeforeChange));
                TryWriteUShort(addresses.WeaponFormerStatsValue, 0);
                TryWriteUShort(addresses.HasChangedBySynth, 0);
                return true;
            }

            return false;
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
