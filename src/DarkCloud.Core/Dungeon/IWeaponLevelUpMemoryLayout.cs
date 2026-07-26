namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="WeaponLevelUpService"/>.
    /// </summary>
    public interface IWeaponLevelUpMemoryLayout
    {
        long MenuModeAddress { get; }
        long PowerPowderMenuAddress { get; }
        long GetWeaponLevelAddress(int index);
        int WeaponSlotSize { get; }
    }
}
