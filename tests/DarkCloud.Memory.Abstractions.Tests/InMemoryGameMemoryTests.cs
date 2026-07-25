using System;
using System.Text;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Memory.Abstractions.Tests
{
    public class InMemoryGameMemoryTests
    {
        private const long Base = 0x20000000L;

        [Fact]
        public void CanWriteAndReadBytes()
        {
            var memory = new InMemoryGameMemory(Base, 32);
            byte[] data = { 1, 2, 3, 4 };

            Assert.True(memory.TryWrite(Base, data, 0, data.Length));

            byte[] read = new byte[4];
            Assert.True(memory.TryRead(Base, read, 0, read.Length));
            Assert.Equal(data, read);
        }

        [Fact]
        public void ReadWrite_RoundTrip()
        {
            var memory = new InMemoryGameMemory(Base, 16);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteUInt32(Base, 0xDEADBEEF);

            Assert.Equal(0xDEADBEEFU, reader.ReadUInt32(Base));
        }

        [Fact]
        public void FirstValidAddress()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.True(memory.TryWrite(Base, new byte[] { 0xAB }, 0, 1));
            Assert.True(memory.TryRead(Base, new byte[1], 0, 1));
        }

        [Fact]
        public void LastValidAddress()
        {
            var memory = new InMemoryGameMemory(Base, 16);
            long last = Base + 15;

            Assert.True(memory.TryWrite(last, new byte[] { 0xAB }, 0, 1));
            Assert.True(memory.TryRead(last, new byte[1], 0, 1));
        }

        [Fact]
        public void NegativeAddress_IsRejected()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.False(memory.TryRead(-1, new byte[1], 0, 1));
            Assert.False(memory.TryWrite(-1, new byte[1], 0, 1));
        }

        [Fact]
        public void AddressBelowBase_IsRejected()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.False(memory.TryRead(Base - 1, new byte[1], 0, 1));
            Assert.False(memory.TryWrite(Base - 1, new byte[1], 0, 1));
        }

        [Fact]
        public void AddressBeyondBuffer_IsRejected()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.False(memory.TryRead(Base + 16, new byte[1], 0, 1));
            Assert.False(memory.TryWrite(Base + 16, new byte[1], 0, 1));
        }

        [Fact]
        public void BoundaryCrossingRead_IsRejected()
        {
            var memory = new InMemoryGameMemory(Base, 16);
            long nearEnd = Base + 14;

            Assert.False(memory.TryRead(nearEnd, new byte[4], 0, 4));
        }

        [Fact]
        public void EmptyOperation_ReturnsTrue()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.True(memory.TryRead(Base, new byte[1], 0, 0));
            Assert.True(memory.TryWrite(Base + 9999, new byte[1], 0, 0));
        }

        [Fact]
        public void OverflowingAddress_IsRejected()
        {
            var memory = new InMemoryGameMemory(Base, 16);
            long overflow = long.MaxValue;

            Assert.False(memory.TryRead(overflow, new byte[1], 0, 1));
        }

        [Fact]
        public void PrimitiveEncoding_LittleEndian()
        {
            var memory = new InMemoryGameMemory(Base, 16);
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteUInt16(Base, 0x0102);
            writer.WriteUInt32(Base + 2, 0x03040506U);

            Assert.Equal(0x0102, reader.ReadUInt16(Base));
            Assert.Equal(0x03040506U, reader.ReadUInt32(Base + 2));
        }

        [Fact]
        public void StringEncoding_RoundTrip()
        {
            var memory = new InMemoryGameMemory(Base, 64);
            var writer = new GameMemoryWriter(memory, Encoding.ASCII);
            var reader = new GameMemoryReader(memory, Encoding.ASCII);

            writer.WriteString(Base, "Hello");

            Assert.Equal("Hello", reader.ReadString(Base, 5));
        }

        [Fact]
        public void Load_PlacesDataAtOffset()
        {
            var memory = new InMemoryGameMemory(Base, 32);
            byte[] data = { 0x11, 0x22, 0x33 };

            memory.Load(data, offset: 4);

            byte[] read = new byte[3];
            Assert.True(memory.TryRead(Base + 4, read, 0, 3));
            Assert.Equal(data, read);
        }

        [Fact]
        public void TryRead_WithInvalidBufferArgs_Throws()
        {
            var memory = new InMemoryGameMemory(Base, 16);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                memory.TryRead(Base, new byte[1], 0, 2));
        }
    }
}
