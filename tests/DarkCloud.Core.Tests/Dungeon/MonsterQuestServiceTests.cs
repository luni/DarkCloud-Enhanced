using System.Collections.Generic;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class MonsterQuestServiceTests
    {
        [Fact]
        public void Process_WhenEnemyMatchesActiveQuest_DecrementsKills()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            long enemyAddress = 0x1020;
            var quests = new List<MonsterQuestDefinition>
            {
                new MonsterQuestDefinition("Test", 0x1040, 0x1041, 0x1042, 2, "Done!"),
            };

            WriteByte(memory, 0x1020 + MonsterQuestService.EnemyTypeOffset, 5);
            WriteByte(memory, 0x1040, 5); // target type
            WriteByte(memory, 0x1041, 3); // kills remaining

            var service = new MonsterQuestService(memory, quests);
            var result = service.Process(enemyAddress, new[] { true });

            Assert.Single(result.ProgressedQuestIndices);
            Assert.Empty(result.CompletedQuestIndices);
            Assert.Equal(2, ReadByte(memory, 0x1041));
        }

        [Fact]
        public void Process_WhenKillsReachZero_CompletesQuest()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            long enemyAddress = 0x1020;
            var quests = new List<MonsterQuestDefinition>
            {
                new MonsterQuestDefinition("Test", 0x1040, 0x1041, 0x1042, 2, "Done!"),
            };

            WriteByte(memory, 0x1020 + MonsterQuestService.EnemyTypeOffset, 5);
            WriteByte(memory, 0x1040, 5);
            WriteByte(memory, 0x1041, 1);

            var service = new MonsterQuestService(memory, quests);
            var result = service.Process(enemyAddress, new[] { true });

            Assert.Single(result.ProgressedQuestIndices);
            Assert.Single(result.CompletedQuestIndices);
            Assert.Equal(0, ReadByte(memory, 0x1041));
            Assert.Equal(2, ReadByte(memory, 0x1042));
        }

        [Fact]
        public void Process_WhenQuestInactive_DoesNothing()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            long enemyAddress = 0x1020;
            var quests = new List<MonsterQuestDefinition>
            {
                new MonsterQuestDefinition("Test", 0x1040, 0x1041, 0x1042, 2, "Done!"),
            };

            WriteByte(memory, 0x1020 + MonsterQuestService.EnemyTypeOffset, 5);
            WriteByte(memory, 0x1040, 5);
            WriteByte(memory, 0x1041, 1);

            var service = new MonsterQuestService(memory, quests);
            var result = service.Process(enemyAddress, new[] { false });

            Assert.Empty(result.ProgressedQuestIndices);
            Assert.Equal(1, ReadByte(memory, 0x1041));
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }
    }
}
