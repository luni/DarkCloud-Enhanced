namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Provides the PS2 memory addresses used to read player identity and
    /// location state. Implementations live in the host application and keep
    /// raw addresses out of <see cref="DarkCloud.Core"/>.
    /// </summary>
    public interface IPlayerPresenceMemoryLayout
    {
        /// <summary>
        /// Address of the byte holding the currently selected character.
        /// </summary>
        long CurrentCharacterAddress { get; }

        /// <summary>
        /// Address of the byte that indicates whether the player is inside a
        /// dungeon floor. A value of 255 indicates the player is not inside a
        /// dungeon floor.
        /// </summary>
        long InDungeonFloorAddress { get; }
    }
}
