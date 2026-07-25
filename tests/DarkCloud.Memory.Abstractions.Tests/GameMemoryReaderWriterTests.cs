using System;
using System.Text;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class GameMemoryReaderWriterTests
    {
        private const long BaseAddress = 0x20000000;

        [Fact]
        public void ReadByte_ReturnsWrittenByte()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteByte(BaseAddress, 0xAB);

            Assert.Equal(0xAB, reader.ReadByte(BaseAddress));
        }

        [Fact]
        public void ReadUInt16_ReturnsLittleEndianValue()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteUInt16(BaseAddress, 0x1234);

            Assert.Equal(0x1234, reader.ReadUInt16(BaseAddress));

            byte[] first = new byte[1];
            byte[] second = new byte[1];
            Assert.True(memory.TryRead(BaseAddress, first, 0, 1));
            Assert.True(memory.TryRead(BaseAddress + 1, second, 0, 1));
            Assert.Equal(0x34, first[0]);
            Assert.Equal(0x12, second[0]);
        }

        [Fact]
        public void ReadInt32_RoundTripPreservesSign()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteInt32(BaseAddress, -42);

            Assert.Equal(-42, reader.ReadInt32(BaseAddress));
        }

        [Fact]
        public void ReadSingle_RoundTripPreservesValue()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteSingle(BaseAddress, 3.14f);

            Assert.Equal(3.14f, reader.ReadSingle(BaseAddress), precision: 5);
        }

        [Fact]
        public void ReadString_ReturnsAsciiString()
        {
            var memory = new FakeGameMemory(BaseAddress, 16);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteString(BaseAddress, "Dark");

            Assert.Equal("Dark", reader.ReadString(BaseAddress, 4));
        }

        [Fact]
        public void ReadBeyondBuffer_Throws()
        {
            var memory = new FakeGameMemory(BaseAddress, 4);
            var reader = new GameMemoryReader(memory);

            Assert.Throws<InvalidOperationException>(() => reader.ReadInt32(BaseAddress + 2));
        }

        [Fact]
        public void WriteBeyondBuffer_Throws()
        {
            var memory = new FakeGameMemory(BaseAddress, 4);
            var writer = new GameMemoryWriter(memory);

            Assert.Throws<InvalidOperationException>(() => writer.WriteInt32(BaseAddress + 2, 1));
        }

        [Fact]
        public void ReadString_WithNegativeLength_Throws()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var reader = new GameMemoryReader(memory);

            Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadString(BaseAddress, -1));
        }

        [Fact]
        public void Writer_UsesProvidedEncoding()
        {
            var memory = new FakeGameMemory(BaseAddress, 8);
            var writer = new GameMemoryWriter(memory, Encoding.UTF8);
            var reader = new GameMemoryReader(memory, Encoding.UTF8);

            writer.WriteString(BaseAddress, "A");

            Assert.Equal("A", reader.ReadString(BaseAddress, 1));
        }
    }
}
