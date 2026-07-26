using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Keeps a rolled mini-boss's stamina timer topped up and clears the rolled
    /// flag when the player enters a back floor.
    /// </summary>
    public sealed class MiniBossStaminaService
    {
        public const int MinimumStamina = 60;
        public const int RestoredStamina = 60000;

        private readonly IGameMemory _memory;
        private readonly IMiniBossStaminaMemoryLayout _layout;

        public MiniBossStaminaService(IGameMemory memory, IMiniBossStaminaMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public bool Update(int enemyNumber, bool miniBossRolled)
        {
            if (miniBossRolled)
            {
                long staminaAddress = _layout.GetStaminaTimerAddress(enemyNumber);
                if (TryReadInt(staminaAddress, out int stamina) && stamina < MinimumStamina)
                {
                    TryWriteInt(staminaAddress, RestoredStamina);
                }
            }

            if (TryReadByte(_layout.BackFloorFlagAddress, out byte backFloorFlag) && backFloorFlag != 0)
            {
                return false;
            }

            return miniBossRolled;
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

        private bool TryReadInt(long address, out int value)
        {
            var buffer = new byte[4];
            if (!_memory.TryRead(address, buffer, 0, 4))
            {
                value = 0;
                return false;
            }

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        private bool TryWriteInt(long address, int value)
        {
            return _memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4);
        }
    }
}
