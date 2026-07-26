using System;
using System.Threading;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Waits for enemies to spawn after entering a dungeon floor and returns
    /// whether any spawned enemy is rendered.
    /// </summary>
    public sealed class SpawnDetectionService
    {
        public const int PollingIntervalMs = 100;
        public const int TimeoutMs = 10000;
        public const byte RenderStatusNotSpawned = 255;
        public const byte HpDefaultValue = 1;

        private readonly IGameMemory _memory;
        private readonly ISpawnDetectionMemoryLayout _layout;

        public SpawnDetectionService(IGameMemory memory, ISpawnDetectionMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public bool WaitForSpawn(int previousFloor, CancellationToken cancellationToken)
        {
            long waitAddress;
            byte waitValue;

            if (previousFloor == 200)
            {
                waitAddress = _layout.Enemy14RenderStatusAddress;
                waitValue = RenderStatusNotSpawned;
            }
            else
            {
                waitAddress = _layout.Enemy14HpAddress;
                waitValue = HpDefaultValue;
            }

            int ms = 0;
            while (TryReadByte(waitAddress, out byte value) && value == waitValue && ms < TimeoutMs)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(PollingIntervalMs)))
                    return false;

                ms += PollingIntervalMs;
            }

            if (TryReadByte(_layout.Enemy0RenderStatusAddress, out byte enemy0RenderStatus))
            {
                return enemy0RenderStatus > 0;
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
    }
}
