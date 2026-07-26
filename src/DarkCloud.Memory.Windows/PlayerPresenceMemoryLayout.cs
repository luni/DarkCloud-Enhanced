using DarkCloud.Core.Players;
using DarkCloudEnhancedMod;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// Maps <see cref="IPlayerPresenceMemoryLayout"/> reads to the legacy
    /// address constants in <see cref="Addresses"/>.
    /// </summary>
    public sealed class PlayerPresenceMemoryLayout : IPlayerPresenceMemoryLayout
    {
        public long CurrentCharacterAddress => 0x21CD9550;

        public long InDungeonFloorAddress => Addresses.checkFloor + 1;
    }
}
