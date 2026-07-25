using System;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Memory.ContractTests
{
    /// <summary>
    /// Reusable contract tests that every <see cref="IGameMemory"/> backend must satisfy.
    /// </summary>
    public abstract class GameMemoryContractTests
    {
        protected abstract IGameMemory CreateMemory();
        protected abstract long BaseAddress { get; }
        protected abstract int Capacity { get; }

        [Fact]
        public void WrittenBytesCanBeReadBack()
        {
            var memory = CreateMemory();
            byte[] data = { 0x01, 0x02, 0x03, 0x04 };

            Assert.True(memory.TryWrite(BaseAddress, data, 0, data.Length));

            byte[] read = new byte[4];
            Assert.True(memory.TryRead(BaseAddress, read, 0, read.Length));
            Assert.Equal(data, read);
        }

        [Fact]
        public void ReadBeyondCapacity_IsRejected()
        {
            var memory = CreateMemory();
            byte[] read = new byte[4];

            Assert.False(memory.TryRead(BaseAddress + Capacity, read, 0, read.Length));
        }

        [Fact]
        public void WriteBeyondCapacity_IsRejected()
        {
            var memory = CreateMemory();
            byte[] data = { 0x01 };

            Assert.False(memory.TryWrite(BaseAddress + Capacity, data, 0, 1));
        }

        [Fact]
        public void EmptyReadWrite_IsAllowed()
        {
            var memory = CreateMemory();

            Assert.True(memory.TryRead(BaseAddress, new byte[1], 0, 0));
            Assert.True(memory.TryWrite(BaseAddress, new byte[1], 0, 0));
        }

        [Fact]
        public void BoundaryCrossingWrite_IsRejected()
        {
            var memory = CreateMemory();
            byte[] data = { 0x01, 0x02 };
            long lastAddress = BaseAddress + Capacity - 1;

            Assert.False(memory.TryWrite(lastAddress, data, 0, data.Length));
        }

        [Fact]
        public void InvalidBufferArguments_Throw()
        {
            var memory = CreateMemory();

            Assert.Throws<ArgumentOutOfRangeException>(() => memory.TryRead(BaseAddress, new byte[1], 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => memory.TryWrite(BaseAddress, new byte[1], 0, 2));
        }

        [Fact]
        public void NullBuffer_Throws()
        {
            var memory = CreateMemory();

            Assert.Throws<ArgumentNullException>(() => memory.TryRead(BaseAddress, null, 0, 1));
            Assert.Throws<ArgumentNullException>(() => memory.TryWrite(BaseAddress, null, 0, 1));
        }

        [Fact]
        public void ReaderWriter_RoundTripPrimitives()
        {
            var memory = CreateMemory();
            var writer = new GameMemoryWriter(memory);
            var reader = new GameMemoryReader(memory);

            writer.WriteUInt32(BaseAddress, 0xDEADBEEF);
            writer.WriteSingle(BaseAddress + 4, 3.14f);

            Assert.Equal(0xDEADBEEFU, reader.ReadUInt32(BaseAddress));
            Assert.Equal(3.14f, reader.ReadSingle(BaseAddress + 4), precision: 5);
        }
    }
}
