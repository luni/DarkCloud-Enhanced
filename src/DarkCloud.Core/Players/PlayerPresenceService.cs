using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Default implementation of <see cref="IPlayerPresenceService"/>. It uses
    /// an <see cref="IPlayerPresenceRepository"/> to read memory and applies the
    /// small amount of domain logic needed to interpret the values.
    /// </summary>
    public sealed class PlayerPresenceService : IPlayerPresenceService
    {
        private readonly IPlayerPresenceRepository _repository;

        public PlayerPresenceService(IGameMemory memory, IPlayerPresenceMemoryLayout layout)
            : this(new PlayerPresenceRepository(memory, layout))
        {
        }

        public PlayerPresenceService(IPlayerPresenceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public CharacterType GetCurrentCharacter()
        {
            if (!_repository.TryReadCurrentCharacter(out CharacterType character))
                return CharacterType.Unknown;

            return character;
        }

        public bool IsInDungeonFloor()
        {
            if (!_repository.TryIsInDungeonFloor(out bool inDungeonFloor))
                return false;

            return inDungeonFloor;
        }
    }
}
