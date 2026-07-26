using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class SideQuestStateServiceTests
    {
        [Fact]
        public void GetState_WhenSambaConditionsMet_ReturnsSambaActive()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSideQuestStateLayout(0x1020, 0x1030, 0x1040);
            WriteByte(memory, 0x1020, 1);

            var service = new SideQuestStateService(memory, layout);
            var state = service.GetState(4, 6);

            Assert.True(state.SambaChallengeActive);
            Assert.False(state.MayorQuestActive);
        }

        [Fact]
        public void GetState_WhenMayorConditionsMet_ReturnsMayorActive()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSideQuestStateLayout(0x1020, 0x1030, 0x1040);
            WriteByte(memory, 0x1030, 1);
            WriteByte(memory, 0x1040, 6);

            var service = new SideQuestStateService(memory, layout);
            var state = service.GetState(6, 5);

            Assert.True(state.MayorQuestActive);
            Assert.False(state.SambaChallengeActive);
        }

        [Fact]
        public void GetState_WhenMayorFloorMismatch_ReturnsMayorInactive()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSideQuestStateLayout(0x1020, 0x1030, 0x1040);
            WriteByte(memory, 0x1030, 1);
            WriteByte(memory, 0x1040, 9);

            var service = new SideQuestStateService(memory, layout);
            var state = service.GetState(6, 5);

            Assert.False(state.MayorQuestActive);
        }

        [Fact]
        public void GetState_WhenNotMayorDungeon_ReturnsMayorInactive()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSideQuestStateLayout(0x1020, 0x1030, 0x1040);
            WriteByte(memory, 0x1030, 1);
            WriteByte(memory, 0x1040, 6);

            var service = new SideQuestStateService(memory, layout);
            var state = service.GetState(5, 5);

            Assert.False(state.MayorQuestActive);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private sealed class FakeSideQuestStateLayout : ISideQuestStateMemoryLayout
        {
            public FakeSideQuestStateLayout(long samba, long mayorFlag, long mayorFloor)
            {
                SambaChallengeFlagAddress = samba;
                MayorQuestFlagAddress = mayorFlag;
                MayorQuestFloorAddress = mayorFloor;
            }

            public long SambaChallengeFlagAddress { get; }
            public long MayorQuestFlagAddress { get; }
            public long MayorQuestFloorAddress { get; }
        }
    }
}
