namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="SpawnDetectionService"/>.
    /// </summary>
    public interface ISpawnDetectionMemoryLayout
    {
        long Enemy14RenderStatusAddress { get; }
        long Enemy14HpAddress { get; }
        long Enemy0RenderStatusAddress { get; }
    }
}
