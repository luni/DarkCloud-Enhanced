using System;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class UngagaDoorServiceTests
    {
        [Fact]
        public void TryFixDoors_WhenTriggerIs150_FixesAllDoorBytes()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeUngagaDoorLayout(0x1020);
            WriteFloat(memory, 0x1020, 150f);

            var service = new UngagaDoorService(memory, layout);
            bool fixedDoors = service.TryFixDoors(3);

            Assert.True(fixedDoors);
            Assert.Equal(30, ReadByte(memory, 0x1030));
            Assert.Equal(50f, ReadFloat(memory, 0x1020));
            Assert.Equal(50f, ReadFloat(memory, 0x1040));
            Assert.Equal(30, ReadByte(memory, 0x1050));
            Assert.Equal(30, ReadByte(memory, 0x1060));
        }

        [Fact]
        public void TryFixDoors_WhenTriggerIsNot150_DoesNothing()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeUngagaDoorLayout(0x1020);
            WriteFloat(memory, 0x1020, 100f);

            var service = new UngagaDoorService(memory, layout);
            bool fixedDoors = service.TryFixDoors(3);

            Assert.False(fixedDoors);
            Assert.Equal(0, ReadByte(memory, 0x1030));
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static float ReadFloat(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[4];
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            return BitConverter.ToSingle(buffer, 0);
        }

        private static void WriteFloat(InMemoryGameMemory memory, long address, float value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4));
        }

        private sealed class FakeUngagaDoorLayout : IUngagaDoorMemoryLayout
        {
            private readonly long _check;

            public FakeUngagaDoorLayout(long check)
            {
                _check = check;
            }

            public long GetDoorCheckAddress(byte dungeon) => _check;
            public long GetDoorByte1Address(byte dungeon) => _check + 0x10;
            public long GetDoorFloat1Address(byte dungeon) => _check;
            public long GetDoorFloat2Address(byte dungeon) => _check + 0x20;
            public long GetDoorByte2Address(byte dungeon) => _check + 0x30;
            public long GetDoorByte3Address(byte dungeon) => _check + 0x40;
        }
    }
}
