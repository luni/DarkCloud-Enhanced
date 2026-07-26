using System;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class SwordOfZeusServiceTests
    {
        [Fact]
        public void ApplyIfSwordOfZeus_WhenSwordOfZeusLeveledUp_AccumulatesThunderAndUpdatesMaxAttack()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSwordOfZeusLayout(0x1020, 0x1030, 0x1040, 0x1050, 0x1060);
            WriteUShort(memory, 0x1020, 296);
            WriteByte(memory, 0x1030, 50);
            WriteByte(memory, 0x1040, 2);
            WriteUShort(memory, 0x1050, 100);

            var service = new SwordOfZeusService(memory, layout);
            service.ApplyIfSwordOfZeus(0);

            Assert.Equal(0, ReadByte(memory, 0x1030));
            Assert.Equal(5, ReadByte(memory, 0x1040));
            Assert.Equal(150, ReadUShort(memory, 0x1050));
            Assert.Equal(274, ReadUShort(memory, 0x1060));
        }

        [Fact]
        public void ApplyIfSwordOfZeus_WhenNotSwordOfZeus_DoesNothing()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSwordOfZeusLayout(0x1020, 0x1030, 0x1040, 0x1050, 0x1060);
            WriteUShort(memory, 0x1020, 123);

            var service = new SwordOfZeusService(memory, layout);
            service.ApplyIfSwordOfZeus(0);

            Assert.Equal(0, ReadUShort(memory, 0x1050));
        }

        [Fact]
        public void ApplyIfSwordOfZeus_WhenStoredThunderNearUshortMax_CapsAtMaxStoredThunder()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeSwordOfZeusLayout(0x1020, 0x1030, 0x1040, 0x1050, 0x1060);
            WriteUShort(memory, 0x1020, 296);
            WriteByte(memory, 0x1030, 100);
            WriteByte(memory, 0x1040, 2);
            WriteUShort(memory, 0x1050, 65530);

            var service = new SwordOfZeusService(memory, layout);
            service.ApplyIfSwordOfZeus(0);

            Assert.Equal(SwordOfZeusService.MaxStoredThunder, ReadUShort(memory, 0x1050));
        }

        [Theory]
        [InlineData(0, 199)]
        [InlineData(100, 249)]
        [InlineData(300, 332)]
        [InlineData(600, 419)]
        [InlineData(1100, 509)]
        [InlineData(2100, 604)]
        public void CalculateMaxAttack_ReturnsExpectedValue(ushort storedThunder, ushort expectedMaxAttack)
        {
            ushort actual = SwordOfZeusService.CalculateMaxAttack(storedThunder);
            Assert.Equal(expectedMaxAttack, actual);
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
            return BitConverter.ToUInt16(buffer, 0);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeSwordOfZeusLayout : ISwordOfZeusMemoryLayout
        {
            private readonly long _id;
            private readonly long _thunder;
            private readonly long _elementHud;
            private readonly long _storedThunder;
            private readonly long _maxAttack;

            public FakeSwordOfZeusLayout(long id, long thunder, long elementHud, long storedThunder, long maxAttack)
            {
                _id = id;
                _thunder = thunder;
                _elementHud = elementHud;
                _storedThunder = storedThunder;
                _maxAttack = maxAttack;
            }

            public long GetWeaponIdAddress(int weaponOffset) => _id;
            public long GetWeaponThunderAddress(int weaponOffset) => _thunder;
            public long GetWeaponElementHudAddress(int weaponOffset) => _elementHud;
            public long StoredThunderAddress => _storedThunder;
            public long MaxAttackAddress => _maxAttack;
        }
    }
}
