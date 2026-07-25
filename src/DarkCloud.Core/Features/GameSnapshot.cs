using DarkCloud.Core.Players;
using DarkCloud.Core.Session;

namespace DarkCloud.Core.Features
{
    /// <summary>
    /// A read-only snapshot of game state provided to features on each tick.
    /// </summary>
    public sealed class GameSnapshot
    {
        public GameSnapshot(
            GameSessionState sessionState,
            CharacterType currentCharacter,
            bool inDungeonFloor)
        {
            SessionState = sessionState;
            CurrentCharacter = currentCharacter;
            InDungeonFloor = inDungeonFloor;
        }

        /// <summary>
        /// The current session state.
        /// </summary>
        public GameSessionState SessionState { get; }

        /// <summary>
        /// The currently selected character, or <see cref="CharacterType.Unknown"/>.
        /// </summary>
        public CharacterType CurrentCharacter { get; }

        /// <summary>
        /// <c>true</c> when the player is inside a dungeon floor.
        /// </summary>
        public bool InDungeonFloor { get; }
    }
}
