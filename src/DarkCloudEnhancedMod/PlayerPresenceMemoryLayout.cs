using DarkCloud.Core.Players;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Maps <see cref="IPlayerPresenceMemoryLayout"/> reads to the legacy
    /// address constants in <see cref="Player"/> and <see cref="Addresses"/>.
    /// </summary>
    internal sealed class PlayerPresenceMemoryLayout : IPlayerPresenceMemoryLayout
    {
        public long CurrentCharacterAddress => Player.currentCharacter;

        public long InDungeonFloorAddress => Addresses.checkFloor + 1;
    }
}
