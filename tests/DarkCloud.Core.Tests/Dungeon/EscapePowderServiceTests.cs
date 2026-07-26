using System.Collections.Generic;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class EscapePowderServiceTests
    {
        [Fact]
        public void TryConsumeEscapePowder_WhenSkipPredicateTrue_DoesNotConsume()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeEscapePowderLayout();
            var service = new EscapePowderService(memory, layout, () => true);

            bool consumed = service.TryConsumeEscapePowder();

            Assert.False(consumed);
        }

        [Fact]
        public void TryConsumeEscapePowder_WhenEscapePowderInFirstSlot_ConsumesOne()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeEscapePowderLayout();
            WriteUShort(memory, 0x1020, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1030, 3);

            var service = new EscapePowderService(memory, layout, () => false);
            bool consumed = service.TryConsumeEscapePowder();

            Assert.True(consumed);
            Assert.Equal(2, ReadByte(memory, 0x1030));
            Assert.Equal(ActiveItemConstants.EscapePowderItemId, ReadUShort(memory, 0x1020));
        }

        [Fact]
        public void TryConsumeEscapePowder_WhenCountReachesZero_ClearsItemId()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeEscapePowderLayout();
            WriteUShort(memory, 0x1020, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1030, 1);

            var service = new EscapePowderService(memory, layout, () => false);
            bool consumed = service.TryConsumeEscapePowder();

            Assert.True(consumed);
            Assert.Equal(0, ReadByte(memory, 0x1030));
            Assert.Equal(ActiveItemConstants.EmptyItemValue, ReadUShort(memory, 0x1020));
        }

        [Fact]
        public void TryConsumeEscapePowder_WhenFirstEmptyButSecondHasPowder_ConsumesFromSecond()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeEscapePowderLayout();
            WriteUShort(memory, 0x1022, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1032, 4);

            var service = new EscapePowderService(memory, layout, () => false);
            bool consumed = service.TryConsumeEscapePowder();

            Assert.True(consumed);
            Assert.Equal(3, ReadByte(memory, 0x1032));
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static ushort ReadUShort(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[2];
            Assert.True(memory.TryRead(address, buffer, 0, 2));
            return System.BitConverter.ToUInt16(buffer, 0);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, System.BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeEscapePowderLayout : IEscapePowderMemoryLayout
        {
            public IReadOnlyList<long> ActiveItemAddresses { get; } = new long[] { 0x1020, 0x1022, 0x1024 };
            public IReadOnlyList<long> ActiveItemCountAddresses { get; } = new long[] { 0x1030, 0x1032, 0x1034 };
        }
    }
}
