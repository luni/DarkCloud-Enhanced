using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Writes the model bytes for Ungaga when the character is swapped to him.
    /// </summary>
    public sealed class UngagaSwapService
    {
        private readonly IGameMemory _memory;
        private readonly IUngagaSwapMemoryLayout _layout;

        public const byte ModelByteValue = 52;

        public UngagaSwapService(IGameMemory memory, IUngagaSwapMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public bool IsModelLoaded(byte dungeon)
        {
            long address = dungeon == 3
                ? _layout.ModelLoadedValueDungeon3Address
                : _layout.ModelLoadedValueOtherAddress;

            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
                return false;

            return BitConverter.ToUInt16(buffer, 0) == _layout.ModelLoadedMagicValue;
        }

        public bool TryWriteUngagaModel()
        {
            if (!TryReadByte(_layout.DungeonIndicatorAddress, out byte dungeon))
                return false;

            if (dungeon == 3)
            {
                TryWriteByte(_layout.ModelWriteAddress1Dungeon3, ModelByteValue);
                TryWriteByte(_layout.ModelWriteAddress2Dungeon3, ModelByteValue);
            }
            else
            {
                TryWriteByte(_layout.ModelWriteAddress1Other, ModelByteValue);
                TryWriteByte(_layout.ModelWriteAddress2Other, ModelByteValue);
            }

            return true;
        }

        private bool TryReadByte(long address, out byte value)
        {
            var buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
            {
                value = 0;
                return false;
            }

            value = buffer[0];
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }
    }
}
