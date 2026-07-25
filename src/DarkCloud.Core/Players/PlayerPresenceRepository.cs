using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Memory-backed implementation of <see cref="IPlayerPresenceRepository"/>.
    /// It maps the raw PS2 addresses used for player identity and location
    /// to the <see cref="IGameMemory"/> contract through an
    /// <see cref="IPlayerPresenceMemoryLayout"/>.
    /// </summary>
    public sealed class PlayerPresenceRepository : IPlayerPresenceRepository
    {
        private readonly IGameMemory _memory;
        private readonly IPlayerPresenceMemoryLayout _layout;

        public PlayerPresenceRepository(IGameMemory memory, IPlayerPresenceMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public bool TryReadCurrentCharacter(out CharacterType character)
        {
            character = CharacterType.Unknown;

            byte[] buffer = new byte[1];
            if (!_memory.TryRead(_layout.CurrentCharacterAddress, buffer, 0, 1))
                return false;

            character = MapByteToCharacterType(buffer[0]);
            return true;
        }

        public bool TryIsInDungeonFloor(out bool inDungeonFloor)
        {
            inDungeonFloor = false;

            byte[] buffer = new byte[1];
            if (!_memory.TryRead(_layout.InDungeonFloorAddress, buffer, 0, 1))
                return false;

            inDungeonFloor = buffer[0] != 255;
            return true;
        }

        private static CharacterType MapByteToCharacterType(byte value)
        {
            switch (value)
            {
                case 0: return CharacterType.Toan;
                case 1: return CharacterType.Xiao;
                case 2: return CharacterType.Goro;
                case 3: return CharacterType.Ruby;
                case 4: return CharacterType.Ungaga;
                case 5: return CharacterType.Osmond;
                default: return CharacterType.Unknown;
            }
        }
    }
}
