using System;

namespace DarkCloud.Core.Weapons
{
    public enum WeaponBalanceValueSize
    {
        Byte,
        UShort,
        Int,
        UInt
    }

    public enum WeaponBalanceOperation
    {
        Set,
        Add
    }

    /// <summary>
    /// Sets or adds a value to a weapon-table stat.
    /// </summary>
    public sealed class WeaponStatBalanceChange : IWeaponBalanceChange
    {
        public WeaponStatBalanceChange(
            int weaponId,
            WeaponCharacter character,
            int baseWeaponId,
            WeaponStat stat,
            int value,
            WeaponBalanceValueSize size,
            WeaponBalanceOperation operation = WeaponBalanceOperation.Set,
            int statOffset = 0)
        {
            WeaponId = weaponId;
            Character = character;
            BaseWeaponId = baseWeaponId;
            Stat = stat;
            Value = value;
            Size = size;
            Operation = operation;
            StatOffset = statOffset;
        }

        public int WeaponId { get; }
        public WeaponCharacter Character { get; }
        public int BaseWeaponId { get; }
        public WeaponStat Stat { get; }
        public int Value { get; }
        public WeaponBalanceValueSize Size { get; }
        public WeaponBalanceOperation Operation { get; }
        public int StatOffset { get; }

        public void Apply(WeaponStatService statService)
        {
            if (statService is null)
                throw new ArgumentNullException(nameof(statService));

            if (Operation == WeaponBalanceOperation.Add)
            {
                ApplyAdd(statService);
                return;
            }

            ApplySet(statService);
        }

        private void ApplySet(WeaponStatService statService)
        {
            switch (Size)
            {
                case WeaponBalanceValueSize.Byte:
                    statService.TryWriteByte(WeaponId, Character, BaseWeaponId, Stat, (byte)Value, StatOffset);
                    break;
                case WeaponBalanceValueSize.UShort:
                    statService.TryWriteUShort(WeaponId, Character, BaseWeaponId, Stat, (ushort)Value, StatOffset);
                    break;
                case WeaponBalanceValueSize.Int:
                    statService.TryWriteInt(WeaponId, Character, BaseWeaponId, Stat, Value, StatOffset);
                    break;
                case WeaponBalanceValueSize.UInt:
                    statService.TryWriteUInt(WeaponId, Character, BaseWeaponId, Stat, (uint)Value, StatOffset);
                    break;
            }
        }

        private void ApplyAdd(WeaponStatService statService)
        {
            if (Size == WeaponBalanceValueSize.UShort)
            {
                if (!statService.TryReadUShort(WeaponId, Character, BaseWeaponId, Stat, out ushort current, StatOffset))
                    return;
                statService.TryWriteUShort(WeaponId, Character, BaseWeaponId, Stat, (ushort)(current + Value), StatOffset);
            }
            else if (Size == WeaponBalanceValueSize.Int)
            {
                if (!statService.TryReadInt(WeaponId, Character, BaseWeaponId, Stat, out int current, StatOffset))
                    return;
                statService.TryWriteInt(WeaponId, Character, BaseWeaponId, Stat, current + Value, StatOffset);
            }
        }
    }
}
