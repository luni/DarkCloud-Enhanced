namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Provides base addresses and offsets for the in-memory weapon database.
    /// </summary>
    public interface IWeaponMemoryLayout
    {
        int WeaponOffset { get; }

        long GetStatBaseAddress(WeaponStat stat);

        int GetCharacterOffset(WeaponCharacter character);

        long LambTransformThresholdAddress { get; }

        long LambStatsThresholdAddress { get; }
    }
}
