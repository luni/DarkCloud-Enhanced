namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Reads the memory-backed values that determine player identity and
    /// location. Implementations isolate the PS2 address layout from the
    /// domain rules in <see cref="IPlayerPresenceService"/>.
    /// </summary>
    public interface IPlayerPresenceRepository
    {
        /// <summary>
        /// Attempts to read the currently selected character.
        /// </summary>
        /// <param name="character">The character read from memory, or <see cref="CharacterType.Unknown"/> on failure.</param>
        /// <returns><c>true</c> if the read succeeded; otherwise, <c>false</c>.</returns>
        bool TryReadCurrentCharacter(out CharacterType character);

        /// <summary>
        /// Attempts to read whether the player is currently inside a dungeon floor.
        /// </summary>
        /// <param name="inDungeonFloor"><c>true</c> when inside a dungeon floor; <c>false</c> on failure or when not in a dungeon floor.</param>
        /// <returns><c>true</c> if the read succeeded; otherwise, <c>false</c>.</returns>
        bool TryIsInDungeonFloor(out bool inDungeonFloor);
    }
}
