using System;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Writes a float value to an absolute memory address.
    /// </summary>
    public sealed class GlobalFloatBalanceChange : IWeaponBalanceChange
    {
        public GlobalFloatBalanceChange(long address, float value)
        {
            Address = address;
            Value = value;
        }

        public long Address { get; }
        public float Value { get; }

        public void Apply(WeaponStatService statService)
        {
            if (statService is null)
                throw new ArgumentNullException(nameof(statService));

            statService.TryWriteFloat(Address, Value);
        }
    }
}
