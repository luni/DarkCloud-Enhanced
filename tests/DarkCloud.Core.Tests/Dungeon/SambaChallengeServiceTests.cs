using System.Collections.Generic;
using System.Threading;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class SambaChallengeServiceTests
    {
        [Fact]
        public void Process_WhenDaggerEquipped_StartsQuest()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x400);
            var layout = new FakeSambaLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, new ushort[] { 257, 258 });
            WriteUShort(memory, 0x1000, 257); // weapon id
            WriteByte(memory, 0x1004, 1); // in dungeon
            WriteByte(memory, 0x1008, 0); // hide hud
            WriteByte(memory, 0x100C, 0); // current ally

            for (int i = 0; i < 8; i++)
            {
                WriteUShort(memory, layout.GetEnemyHpAddress(i), 1); // enemies alive
            }

            var service = new SambaChallengeService(memory, layout);
            bool[] monstersDead = new bool[8];
            var result = service.Process(false, false, true, monstersDead, CancellationToken.None);

            Assert.True(result.QuestCheck);
            Assert.True(result.QuestActive);
            Assert.Single(result.Messages);
            Assert.Equal(0, ReadInt(memory, 0x1014));
        }

        [Fact]
        public void Process_WhenWrongWeapon_StartFailed()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x400);
            var layout = new FakeSambaLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, new ushort[] { 257, 258 });
            WriteUShort(memory, 0x1000, 1);
            WriteByte(memory, 0x1004, 1);
            WriteByte(memory, 0x1008, 0);
            WriteByte(memory, 0x100C, 0);

            var service = new SambaChallengeService(memory, layout);
            bool[] monstersDead = new bool[8];
            var result = service.Process(false, false, true, monstersDead, CancellationToken.None);

            Assert.True(result.QuestCheck);
            Assert.False(result.QuestActive);
            Assert.Single(result.Messages);
        }

        [Fact]
        public void Process_WhenAllEnemiesKilled_CompletesQuest()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x400);
            var layout = new FakeSambaLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, new ushort[] { 257, 258 });
            WriteUShort(memory, 0x1000, 257);
            WriteByte(memory, 0x1004, 1); // in dungeon

            for (int i = 0; i < 8; i++)
            {
                WriteUShort(memory, layout.GetEnemyHpAddress(i), 0); // all dead
            }

            var service = new SambaChallengeService(memory, layout);
            bool[] monstersDead = new bool[8];
            var result = service.Process(true, true, true, monstersDead, CancellationToken.None);

            Assert.Single(result.Messages);
            Assert.False(result.Quest);
            Assert.Equal(1, ReadByte(memory, 0x1018));
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static int ReadInt(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[4];
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            return System.BitConverter.ToInt32(buffer, 0);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, System.BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeSambaLayout : ISambaChallengeMemoryLayout
        {
            private readonly long _weaponId;
            private readonly long _inDungeon;
            private readonly long _hideHud;
            private readonly long _ally;
            private readonly long _anim;
            private readonly long _timer;
            private readonly long _completion;

            public FakeSambaLayout(long weaponId, long inDungeon, long hideHud, long ally, long anim, long timer, long completion, IReadOnlyList<ushort> allowed)
            {
                _weaponId = weaponId;
                _inDungeon = inDungeon;
                _hideHud = hideHud;
                _ally = ally;
                _anim = anim;
                _timer = timer;
                _completion = completion;
                AllowedWeaponIds = allowed;
            }

            public long CurrentWeaponIdAddress => _weaponId;
            public long InDungeonFlagAddress => _inDungeon;
            public long HideHudAddress => _hideHud;
            public long CurrentAllyAddress => _ally;
            public long AnimationIdAddress => _anim;
            public long QuestTimerAddress => _timer;
            public long CompletionAddress => _completion;
            public IReadOnlyList<ushort> AllowedWeaponIds { get; }
            public int EnemyHpSlotSize => 0x10;
            public int EnemyCount => 8;

            public long GetEnemyHpAddress(int index)
            {
                return 0x1100 + (index * EnemyHpSlotSize);
            }
        }
    }
}
