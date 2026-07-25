using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Memory-backed implementation of <see cref="IPlayerStateRepository"/>.
    /// It maps the raw PS2 addresses used for player state to the
    /// <see cref="IGameMemory"/> contract through an
    /// <see cref="IPlayerCharacterMemoryLayout"/>.
    /// </summary>
    public sealed class PlayerStateRepository : IPlayerStateRepository
    {
        // NTSC addresses for the current character and dungeon-floor flag.
        // Region translation is applied by the underlying IGameMemory implementation.
        private const long CurrentCharacterAddress = 0x21CD9550L;
        private const long InDungeonFloorAddress = 0x21CD954FL;

        private readonly IGameMemory _memory;
        private readonly IPlayerCharacterMemoryLayout _layout;

        public PlayerStateRepository(IGameMemory memory, IPlayerCharacterMemoryLayout layout = null)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout;
        }

        public bool TryReadCurrentCharacter(out CharacterType character)
        {
            character = CharacterType.Unknown;

            byte[] buffer = new byte[1];
            if (!_memory.TryRead(CurrentCharacterAddress, buffer, 0, 1))
                return false;

            character = MapByteToCharacterType(buffer[0]);
            return true;
        }

        public bool TryIsInDungeonFloor(out bool inDungeonFloor)
        {
            inDungeonFloor = false;

            byte[] buffer = new byte[1];
            if (!_memory.TryRead(InDungeonFloorAddress, buffer, 0, 1))
                return false;

            // 255 indicates the player is not inside a dungeon floor.
            inDungeonFloor = buffer[0] != 255;
            return true;
        }

        public bool TryReadUInt16(CharacterType character, PlayerCharacterField field, out ushort value)
        {
            value = 0;

            long address = _layout.GetAddress(character, field, false);
            byte[] buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
                return false;

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        public bool TryReadInt32(CharacterType character, PlayerCharacterField field, out int value)
        {
            value = 0;

            long address = _layout.GetAddress(character, field, false);
            byte[] buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
                return false;

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        public bool TryReadSingle(CharacterType character, PlayerCharacterField field, out float value)
        {
            value = 0;

            long address = _layout.GetAddress(character, field, false);
            byte[] buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
                return false;

            value = BitConverter.ToSingle(buffer, 0);
            return true;
        }

        public bool TryReadByte(CharacterType character, PlayerCharacterField field, out byte value)
        {
            value = 0;

            long address = _layout.GetAddress(character, field, false);
            byte[] buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
                return false;

            value = buffer[0];
            return true;
        }

        public bool TryWriteUInt16(CharacterType character, PlayerCharacterField field, ushort value)
        {
            long address = _layout.GetAddress(character, field, true);
            byte[] bytes = BitConverter.GetBytes(value);
            return _memory.TryWrite(address, bytes, 0, bytes.Length);
        }

        public bool TryWriteInt32(CharacterType character, PlayerCharacterField field, int value)
        {
            long address = _layout.GetAddress(character, field, true);
            byte[] bytes = BitConverter.GetBytes(value);
            return _memory.TryWrite(address, bytes, 0, bytes.Length);
        }

        public bool TryWriteSingle(CharacterType character, PlayerCharacterField field, float value)
        {
            long address = _layout.GetAddress(character, field, true);
            byte[] bytes = BitConverter.GetBytes(value);
            return _memory.TryWrite(address, bytes, 0, bytes.Length);
        }

        public bool TryWriteByte(CharacterType character, PlayerCharacterField field, byte value)
        {
            long address = _layout.GetAddress(character, field, true);
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
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
