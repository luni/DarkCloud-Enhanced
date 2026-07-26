using System;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Determines which optional side quests are active based on the current
    /// dungeon, floor, and in-memory quest flags.
    /// </summary>
    public sealed class SideQuestStateService
    {
        public const byte SambaDungeon = 4;
        public const byte SambaFloor = 6;
        public const byte MayorDungeon = 6;

        private readonly IGameMemory _memory;
        private readonly ISideQuestStateMemoryLayout _layout;

        public SideQuestStateService(IGameMemory memory, ISideQuestStateMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public SideQuestState GetState(byte currentDungeon, byte currentFloor)
        {
            var state = new SideQuestState();

            state.SambaChallengeActive = currentDungeon == SambaDungeon
                && currentFloor == SambaFloor
                && TryReadByte(_layout.SambaChallengeFlagAddress, out byte sambaFlag)
                && sambaFlag == 1;

            if (currentDungeon == MayorDungeon)
            {
                if (TryReadByte(_layout.MayorQuestFlagAddress, out byte mayorFlag) && mayorFlag == 1)
                {
                    if (TryReadByte(_layout.MayorQuestFloorAddress, out byte mayorFloor))
                    {
                        state.MayorQuestActive = currentFloor == mayorFloor - 1;
                    }
                }
            }

            return state;
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
