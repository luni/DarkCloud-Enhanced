using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Fixes Ungaga's dungeon doors when the trigger float is still at its
    /// initial value. The memory layout is supplied by the host so the service
    /// can be exercised against an in-memory buffer in tests.
    /// </summary>
    public sealed class UngagaDoorService
    {
        private readonly IGameMemory _memory;
        private readonly IUngagaDoorMemoryLayout _layout;

        public const float TriggerValue = 150f;
        public const byte DoorByteValue = 30;
        public const float DoorFloatValue = 50f;

        public UngagaDoorService(IGameMemory memory, IUngagaDoorMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public bool TryFixDoors(byte dungeon)
        {
            if (!TryReadFloat(_layout.GetDoorCheckAddress(dungeon), out float value))
                return false;

            if (Math.Abs(value - TriggerValue) > 0.0001f)
                return false;

            TryWriteByte(_layout.GetDoorByte1Address(dungeon), DoorByteValue);
            TryWriteFloat(_layout.GetDoorFloat1Address(dungeon), DoorFloatValue);
            TryWriteFloat(_layout.GetDoorFloat2Address(dungeon), DoorFloatValue);
            TryWriteByte(_layout.GetDoorByte2Address(dungeon), DoorByteValue);
            TryWriteByte(_layout.GetDoorByte3Address(dungeon), DoorByteValue);
            return true;
        }

        private bool TryReadFloat(long address, out float value)
        {
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToSingle(buffer, 0);
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        private bool TryWriteFloat(long address, float value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }
    }
}
