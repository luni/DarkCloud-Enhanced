namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory address used by <see cref="MiniBossMessageService"/>.
    /// </summary>
    public interface IMiniBossMessageMemoryLayout
    {
        long HideHudAddress { get; }
    }
}
