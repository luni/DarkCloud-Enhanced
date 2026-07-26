using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Consumes an escape powder from an active item slot when the player uses
    /// the dungeon-leave shortcut. The predicate lets the host decide whether
    /// an escape powder should be treated as a quest reward and ignored.
    /// </summary>
    public sealed class EscapePowderService
    {
        private readonly IGameMemory _memory;
        private readonly IEscapePowderMemoryLayout _layout;
        private readonly Func<bool> _shouldSkipConsumption;

        public EscapePowderService(IGameMemory memory, IEscapePowderMemoryLayout layout, Func<bool> shouldSkipConsumption)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
            _shouldSkipConsumption = shouldSkipConsumption ?? throw new ArgumentNullException(nameof(shouldSkipConsumption));
        }

        public bool TryConsumeEscapePowder()
        {
            if (_shouldSkipConsumption())
                return false;

            for (int i = 0; i < _layout.ActiveItemAddresses.Count; i++)
            {
                long itemAddress = _layout.ActiveItemAddresses[i];
                if (!TryReadUShort(itemAddress, out ushort itemId))
                    continue;

                if (itemId != ActiveItemConstants.EscapePowderItemId)
                    continue;

                long countAddress = _layout.ActiveItemCountAddresses[i];
                if (!TryReadByte(countAddress, out byte count))
                    continue;

                if (count == 0)
                    continue;

                count--;
                TryWriteByte(countAddress, count);

                if (count == 0)
                {
                    TryWriteUShort(itemAddress, ActiveItemConstants.EmptyItemValue);
                }

                return true;
            }

            return false;
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
