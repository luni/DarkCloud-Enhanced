namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="FloorSelectionService"/>.
    /// </summary>
    public interface IFloorSelectionMemoryLayout
    {
        long ButtonInputsAddress { get; }
        long GildaAddress { get; }
        long DungeonDebugMenuAddress { get; }
        long DungeonModeAddress { get; }
    }
}
