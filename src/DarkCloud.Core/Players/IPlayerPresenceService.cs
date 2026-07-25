namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Domain service for player presence and identity. It applies the game
    /// rules that interpret raw memory values (for example, mapping a byte to
    /// a character or determining whether a value means "in town").
    /// </summary>
    public interface IPlayerPresenceService
    {
        /// <summary>
        /// Returns the currently selected character, or <see cref="CharacterType.Unknown"/>
        /// when the value cannot be read or is not a valid character ID.
        /// </summary>
        CharacterType GetCurrentCharacter();

        /// <summary>
        /// Returns <c>true</c> when the player is inside a dungeon floor.
        /// </summary>
        bool IsInDungeonFloor();
    }
}
