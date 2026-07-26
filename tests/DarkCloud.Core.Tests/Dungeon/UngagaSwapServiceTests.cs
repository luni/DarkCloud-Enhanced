using System;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class UngagaSwapServiceTests
    {
        [Fact]
        public void IsModelLoaded_WhenValueMatchesMagicValue_ReturnsTrue()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeUngagaSwapLayout();
            WriteUShort(memory, layout.ModelLoadedValueDungeon3Address, 12850);

            var service = new UngagaSwapService(memory, layout);
            bool loaded = service.IsModelLoaded(3);

            Assert.True(loaded);
        }

        [Fact]
        public void TryWriteUngagaModel_WhenDungeonIs3_WritesModelBytes()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeUngagaSwapLayout();
            WriteByte(memory, layout.DungeonIndicatorAddress, 3);

            var service = new UngagaSwapService(memory, layout);
            bool written = service.TryWriteUngagaModel();

            Assert.True(written);
            Assert.Equal(52, ReadByte(memory, layout.ModelWriteAddress1Dungeon3));
            Assert.Equal(52, ReadByte(memory, layout.ModelWriteAddress2Dungeon3));
        }

        [Fact]
        public void TryWriteUngagaModel_WhenDungeonIsOther_WritesOtherModelBytes()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeUngagaSwapLayout();
            WriteByte(memory, layout.DungeonIndicatorAddress, 5);

            var service = new UngagaSwapService(memory, layout);
            bool written = service.TryWriteUngagaModel();

            Assert.True(written);
            Assert.Equal(52, ReadByte(memory, layout.ModelWriteAddress1Other));
            Assert.Equal(52, ReadByte(memory, layout.ModelWriteAddress2Other));
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
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeUngagaSwapLayout : IUngagaSwapMemoryLayout
        {
            public long DungeonIndicatorAddress => 0x1010;
            public long ModelLoadedValueDungeon3Address => 0x1020;
            public long ModelLoadedValueOtherAddress => 0x1030;
            public long ModelWriteAddress1Dungeon3 => 0x1040;
            public long ModelWriteAddress2Dungeon3 => 0x1041;
            public long ModelWriteAddress1Other => 0x1050;
            public long ModelWriteAddress2Other => 0x1051;
            public ushort ModelLoadedMagicValue => 12850;
        }
    }
}
