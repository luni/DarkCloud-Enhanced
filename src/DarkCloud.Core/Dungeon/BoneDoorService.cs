using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Reads and writes the bone-door bypass flag for a dungeon.
    /// </summary>
    public sealed class BoneDoorService
    {
        private readonly IGameMemory _memory;
        private readonly IDungeonMemoryLayout _layout;

        public BoneDoorService(IGameMemory memory, IDungeonMemoryLayout layout)
        {
            _memory = memory ?? throw new System.ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new System.ArgumentNullException(nameof(layout));
        }

        public bool IsOpen()
        {
            var buffer = new byte[1];
            if (!_memory.TryRead(_layout.BoneDoorOpenTypeAddress, buffer, 0, 1))
                return false;

            return buffer[0] == 5;
        }

        public bool SetOpen(bool open)
        {
            var buffer = new byte[1] { open ? (byte)5 : (byte)21 };
            return _memory.TryWrite(_layout.BoneDoorOpenTypeAddress, buffer, 0, 1);
        }
    }
}
