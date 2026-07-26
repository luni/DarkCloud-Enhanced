using System;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class MiniBossStaminaServiceTests
    {
        [Fact]
        public void Update_WhenStaminaBelowMinimum_RestoresStamina()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeMiniBossStaminaLayout(0x1020, 0x1080);
            WriteInt(memory, 0x1040, 30);
            WriteByte(memory, 0x1080, 0);

            var service = new MiniBossStaminaService(memory, layout);
            bool rolled = service.Update(2, true);

            Assert.True(rolled);
            Assert.Equal(60000, ReadInt(memory, 0x1040));
        }

        [Fact]
        public void Update_WhenBackFloorFlagSet_ClearsRolled()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeMiniBossStaminaLayout(0x1020, 0x1080);
            WriteByte(memory, 0x1080, 1);

            var service = new MiniBossStaminaService(memory, layout);
            bool rolled = service.Update(2, true);

            Assert.False(rolled);
        }

        [Fact]
        public void Update_WhenNotRolled_DoesNotReadStamina()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeMiniBossStaminaLayout(0x1020, 0x1080);
            WriteByte(memory, 0x1080, 0);

            var service = new MiniBossStaminaService(memory, layout);
            bool rolled = service.Update(2, false);

            Assert.False(rolled);
            Assert.Equal(0, ReadInt(memory, 0x1020));
        }

        private static int ReadInt(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[4];
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            return BitConverter.ToInt32(buffer, 0);
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static void WriteInt(InMemoryGameMemory memory, long address, int value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4));
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private sealed class FakeMiniBossStaminaLayout : IMiniBossStaminaMemoryLayout
        {
            private readonly long _staminaBase;
            private readonly long _backFloorFlag;

            public FakeMiniBossStaminaLayout(long staminaBase, long backFloorFlag)
            {
                _staminaBase = staminaBase;
                _backFloorFlag = backFloorFlag;
            }

            public long GetStaminaTimerAddress(int enemyNumber)
            {
                return _staminaBase + (0x10 * enemyNumber);
            }

            public long BackFloorFlagAddress => _backFloorFlag;
        }
    }
}
