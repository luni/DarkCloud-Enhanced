namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the absolute memory addresses used by <see cref="UngagaSwapService"/>.
    /// </summary>
    public interface IUngagaSwapMemoryLayout
    {
        long DungeonIndicatorAddress { get; }
        long CurrentCharacterCursorAddress { get; }
        long ModelLoadedValueDungeon3Address { get; }
        long ModelLoadedValueOtherAddress { get; }
        long ModelWriteAddress1Dungeon3 { get; }
        long ModelWriteAddress2Dungeon3 { get; }
        long ModelWriteAddress1Other { get; }
        long ModelWriteAddress2Other { get; }
        ushort ModelLoadedMagicValue { get; }
    }
}
