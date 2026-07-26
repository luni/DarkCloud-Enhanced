using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Handles the debug floor-selection input: pressing Circle opens the debug
    /// menu, releasing Circle commits the current gilda value.
    /// </summary>
    public sealed class FloorSelectionService
    {
        public const ushort CircleButton = 0x0020; // CheatCodes.InputBuffer.Button.Circle
        public const ushort DebugMenuValue = 170;
        public const byte DungeonModeValue = 1;

        private readonly IGameMemory _memory;
        private readonly IFloorSelectionMemoryLayout _layout;

        public FloorSelectionService(IGameMemory memory, IFloorSelectionMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public void Update(ref bool circlePressed, out ushort currentGilda)
        {
            currentGilda = 0;

            if (!TryReadUShort(_layout.ButtonInputsAddress, out ushort buttonInputs))
                return;

            if (!circlePressed)
            {
                if (buttonInputs == CircleButton)
                {
                    circlePressed = true;
                }
            }
            else
            {
                if (buttonInputs != CircleButton)
                {
                    if (TryReadUShort(_layout.GildaAddress, out currentGilda))
                    {
                        // value read successfully
                    }

                    TryWriteUShort(_layout.DungeonDebugMenuAddress, DebugMenuValue);
                    TryWriteByte(_layout.DungeonModeAddress, DungeonModeValue);
                    circlePressed = false;
                }
            }
        }

        private bool TryReadUShort(long address, out ushort value)
        {
            var buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        private bool TryWriteByte(long address, byte value)
        {
            return _memory.TryWrite(address, new byte[] { value }, 0, 1);
        }

        private bool TryWriteUShort(long address, ushort value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2);
        }
    }
}
