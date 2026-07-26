using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class BoneDoorServiceTests
    {
        private const long BoneDoorAddress = 0x20931768L;

        [Fact]
        public void IsOpen_WhenValueIsFive_ReturnsTrue()
        {
            var memory = new InMemoryGameMemory(BoneDoorAddress, 0x100);
            memory.TryWrite(BoneDoorAddress, new byte[] { 5 }, 0, 1);
            var service = new BoneDoorService(memory, new TestLayout());

            Assert.True(service.IsOpen());
        }

        [Fact]
        public void IsOpen_WhenValueIsDefault_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory(BoneDoorAddress, 0x100);
            memory.TryWrite(BoneDoorAddress, new byte[] { 21 }, 0, 1);
            var service = new BoneDoorService(memory, new TestLayout());

            Assert.False(service.IsOpen());
        }

        [Fact]
        public void SetOpen_WritesFiveWhenOpen()
        {
            var memory = new InMemoryGameMemory(BoneDoorAddress, 0x100);
            var service = new BoneDoorService(memory, new TestLayout());

            Assert.True(service.SetOpen(true));

            var buffer = new byte[1];
            Assert.True(memory.TryRead(BoneDoorAddress, buffer, 0, 1));
            Assert.Equal(5, buffer[0]);
        }

        [Fact]
        public void SetOpen_WritesTwentyOneWhenClosed()
        {
            var memory = new InMemoryGameMemory(BoneDoorAddress, 0x100);
            var service = new BoneDoorService(memory, new TestLayout());

            Assert.True(service.SetOpen(false));

            var buffer = new byte[1];
            Assert.True(memory.TryRead(BoneDoorAddress, buffer, 0, 1));
            Assert.Equal(21, buffer[0]);
        }

        [Fact]
        public void IsOpen_WhenReadFails_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory(0x0, 0x1);
            var service = new BoneDoorService(memory, new TestLayout());

            Assert.False(service.IsOpen());
        }

        private sealed class TestLayout : IDungeonMemoryLayout
        {
            public long BoneDoorOpenTypeAddress => BoneDoorAddress;
        }
    }
}
