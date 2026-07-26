using System.Threading;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class MayorQuestServiceTests
    {
        [Fact]
        public void Process_WhenCorrectAlly_StartsQuest()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x400);
            var layout = new FakeMayorLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018);
            WriteByte(memory, 0x1000, 1); // in dungeon
            WriteByte(memory, 0x1004, 0); // hide hud
            WriteByte(memory, 0x1008, 3); // current ally
            WriteByte(memory, 0x100C, 3); // expected ally

            for (int i = 0; i < 8; i++)
            {
                WriteUShort(memory, layout.GetEnemyHpAddress(i), 1); // enemies alive
            }

            var service = new MayorQuestService(memory, layout);
            bool[] monstersDead = new bool[8];
            var result = service.Process(false, false, true, monstersDead, CancellationToken.None);

            Assert.True(result.QuestCheck);
            Assert.True(result.QuestActive);
            Assert.Single(result.Messages);
        }

        [Fact]
        public void Process_WhenAllEnemiesKilled_CompletesQuest()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x400);
            var layout = new FakeMayorLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018);
            WriteByte(memory, 0x1000, 1); // in dungeon

            for (int i = 0; i < 8; i++)
            {
                WriteUShort(memory, layout.GetEnemyHpAddress(i), 0); // all dead
            }

            var service = new MayorQuestService(memory, layout);
            bool[] monstersDead = new bool[8];
            var result = service.Process(true, true, true, monstersDead, CancellationToken.None);

            Assert.Single(result.Messages);
            Assert.False(result.Quest);
            Assert.Equal(2, ReadByte(memory, 0x1018));
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

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, System.BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeMayorLayout : IMayorQuestMemoryLayout
        {
            public FakeMayorLayout(long inDungeon, long hideHud, long currentAlly, long expectedAlly, long anim, long timer, long completion)
            {
                InDungeonFlagAddress = inDungeon;
                HideHudAddress = hideHud;
                CurrentAllyAddress = currentAlly;
                ExpectedAllyAddress = expectedAlly;
                AnimationIdAddress = anim;
                QuestTimerAddress = timer;
                CompletionAddress = completion;
            }

            public long InDungeonFlagAddress { get; }
            public long HideHudAddress { get; }
            public long CurrentAllyAddress { get; }
            public long ExpectedAllyAddress { get; }
            public long AnimationIdAddress { get; }
            public long QuestTimerAddress { get; }
            public long CompletionAddress { get; }
            public int EnemyHpSlotSize => 0x10;
            public int EnemyCount => 8;

            public long GetEnemyHpAddress(int index)
            {
                return 0x1100 + (index * EnemyHpSlotSize);
            }
        }
    }
}
