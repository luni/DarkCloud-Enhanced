using System;
using System.Collections.Generic;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Tracks monster-kill side quests. When the killed enemy type matches a
    /// quest target the remaining kill count is decremented and, on completion,
    /// the completion byte is written and the quest index is reported.
    /// </summary>
    public sealed class MonsterQuestService
    {
        public const int EnemyTypeOffset = 0x1E;

        private readonly IGameMemory _memory;
        private readonly IReadOnlyList<MonsterQuestDefinition> _quests;

        public MonsterQuestService(IGameMemory memory, IReadOnlyList<MonsterQuestDefinition> quests)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _quests = quests ?? throw new ArgumentNullException(nameof(quests));
        }

        public MonsterQuestResult Process(long currentEnemyAddress, IReadOnlyList<bool> activeQuests)
        {
            if (activeQuests == null)
                throw new ArgumentNullException(nameof(activeQuests));

            var progressed = new List<int>();
            var completed = new List<int>();

            long enemyTypeAddress = currentEnemyAddress + EnemyTypeOffset;
            if (!TryReadByte(enemyTypeAddress, out byte enemyType))
                return new MonsterQuestResult(progressed, completed);

            for (int i = 0; i < _quests.Count; i++)
            {
                if (i >= activeQuests.Count || !activeQuests[i])
                    continue;

                MonsterQuestDefinition quest = _quests[i];

                if (!TryReadByte(quest.TargetTypeAddress, out byte targetType))
                    continue;

                if (enemyType != targetType)
                    continue;

                if (!TryReadByte(quest.KillsRemainingAddress, out byte killsRemaining))
                    continue;

                if (killsRemaining == 0)
                    continue;

                progressed.Add(i);
                killsRemaining--;
                TryWriteByte(quest.KillsRemainingAddress, killsRemaining);

                if (killsRemaining == 0)
                {
                    TryWriteByte(quest.CompletionAddress, quest.CompletionValue);
                    completed.Add(i);
                }
            }

            return new MonsterQuestResult(progressed, completed);
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
