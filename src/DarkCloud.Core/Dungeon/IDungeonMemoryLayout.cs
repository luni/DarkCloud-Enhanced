namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Memory addresses used by dungeon services. Implementations are supplied
    /// by the host process so the Core domain remains platform and process
    /// agnostic.
    /// </summary>
    public interface IDungeonMemoryLayout
    {
        long BoneDoorOpenTypeAddress { get; }
    }
}
