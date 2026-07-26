namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="SwordOfZeusService"/>.
    /// </summary>
    public interface ISwordOfZeusMemoryLayout
    {
        long GetWeaponIdAddress(int weaponOffset);
        long GetWeaponThunderAddress(int weaponOffset);
        long GetWeaponElementHudAddress(int weaponOffset);
        long StoredThunderAddress { get; }
        long MaxAttackAddress { get; }
    }
}
