using System;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Writes a double value to an absolute memory address.
    /// </summary>
    public sealed class GlobalDoubleBalanceChange : IWeaponBalanceChange
    {
        public GlobalDoubleBalanceChange(long address, double value)
        {
            Address = address;
            Value = value;
        }

        public long Address { get; }
        public double Value { get; }

        public void Apply(WeaponStatService statService)
        {
            if (statService is null)
                throw new ArgumentNullException(nameof(statService));

            statService.TryWriteDouble(Address, Value);
        }
    }
}
