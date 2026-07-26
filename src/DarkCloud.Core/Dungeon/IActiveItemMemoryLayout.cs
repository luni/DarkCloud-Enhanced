namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="ActiveItemService"/>.
    /// </summary>
    public interface IActiveItemMemoryLayout
    {
        long ButtonInputsAddress { get; }
        long ActiveItemUsableFlagAddress { get; }
        long ActiveItemUsableIntAddress { get; }
        long CurrentSlotAddress { get; }
        long ActiveItemBaseAddress { get; }
        int ActiveItemSlotSize { get; }
        long AnimationIdAddress { get; }
        long EscapeFlagAddress { get; }
        long CurrentCharacterAddress { get; }
        long CurrentWeaponSlotAddress { get; }
        long CurrentWeaponMaxWhpAddress { get; }
        long GetPowderCountAddress(int slot);
        long GetCharacterWeaponWhpAddress(int character, int weaponSlot);
    }
}
