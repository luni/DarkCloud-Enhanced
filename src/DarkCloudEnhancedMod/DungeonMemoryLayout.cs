using DarkCloud.Core.Dungeon;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Provides the memory addresses used by dungeon domain services for the
    /// Dark Cloud Enhanced mod.
    /// </summary>
    internal sealed class DungeonMemoryLayout : IDungeonMemoryLayout
    {
        public long BoneDoorOpenTypeAddress => Addresses.BoneDoorOpenType;
    }
}
