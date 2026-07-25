using System;
using System.Collections.Generic;
using DarkCloud.Core.Players;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Players
{
    public class PlayerStateServiceTests
    {
        private static PlayerStateService CreateService(IGameMemory memory)
        {
            var layout = new TestCharacterLayout();
            var repository = new PlayerStateRepository(memory, layout);
            return new PlayerStateService(repository);
        }

        [Fact]
        public void GetHp_ReadsUInt16Value()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestCharacterLayout();
            ushort expected = 12345;
            memory.Load(BitConverter.GetBytes(expected), (int)(layout.GetAddress(CharacterType.Toan, PlayerCharacterField.Hp, false) - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.Equal(expected, service.GetHp(CharacterType.Toan));
        }

        [Fact]
        public void SetHp_WritesUInt16Value()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetHp(CharacterType.Toan, 12345);

            byte[] buffer = new byte[2];
            long address = new TestCharacterLayout().GetAddress(CharacterType.Toan, PlayerCharacterField.Hp, true);
            Assert.True(memory.TryRead(address, buffer, 0, 2));
            Assert.Equal(12345, BitConverter.ToUInt16(buffer, 0));
        }

        [Fact]
        public void SetMaxHp_ForToan_WritesInt32()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetMaxHp(CharacterType.Toan, 50000);

            byte[] buffer = new byte[4];
            long address = new TestCharacterLayout().GetAddress(CharacterType.Toan, PlayerCharacterField.MaxHp, true);
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            Assert.Equal(50000, BitConverter.ToInt32(buffer, 0));
        }

        [Fact]
        public void SetMaxHp_ForOtherCharacters_WritesUInt16()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetMaxHp(CharacterType.Goro, 50000);

            byte[] buffer = new byte[4];
            long address = new TestCharacterLayout().GetAddress(CharacterType.Goro, PlayerCharacterField.MaxHp, true);
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            // Only the first two bytes should be written; the remaining bytes are untouched (zero).
            Assert.Equal(50000, BitConverter.ToUInt16(buffer, 0));
            Assert.Equal(0, BitConverter.ToUInt16(buffer, 2));
        }

        [Fact]
        public void GetMaxThirst_ForToan_ReadsThirstAddress()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestCharacterLayout();
            float expected = 42.5f;
            memory.Load(BitConverter.GetBytes(expected), (int)(layout.GetAddress(CharacterType.Toan, PlayerCharacterField.Thirst, false) - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.Equal(expected, service.GetMaxThirst(CharacterType.Toan));
        }

        [Fact]
        public void SetMaxThirst_ForToan_WritesMaxThirstAddress()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetMaxThirst(CharacterType.Toan, 42.5f);

            byte[] buffer = new byte[4];
            long address = new TestCharacterLayout().GetAddress(CharacterType.Toan, PlayerCharacterField.MaxThirst, true);
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            Assert.Equal(42.5f, BitConverter.ToSingle(buffer, 0));
        }

        [Theory]
        [InlineData("freeze", (ushort)4)]
        [InlineData("stamina", (ushort)8)]
        [InlineData("poison", (ushort)16)]
        [InlineData("curse", (ushort)32)]
        [InlineData("goo", (ushort)64)]
        public void SetStatus_WritesStatusAndTimer(string type, ushort expectedStatus)
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetStatus(CharacterType.Toan, type, 1800);

            byte[] statusBuffer = new byte[2];
            byte[] timerBuffer = new byte[2];
            var layout = new TestCharacterLayout();
            Assert.True(memory.TryRead(layout.GetAddress(CharacterType.Toan, PlayerCharacterField.Status, true), statusBuffer, 0, 2));
            Assert.True(memory.TryRead(layout.GetAddress(CharacterType.Toan, PlayerCharacterField.StatusTimer, true), timerBuffer, 0, 2));

            Assert.Equal(expectedStatus, BitConverter.ToUInt16(statusBuffer, 0));
            Assert.Equal(1800, BitConverter.ToUInt16(timerBuffer, 0));
        }

        [Fact]
        public void SetStatus_UnknownType_DoesNothing()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            service.SetStatus(CharacterType.Toan, "unknown", 1800);

            byte[] buffer = new byte[2];
            long address = new TestCharacterLayout().GetAddress(CharacterType.Toan, PlayerCharacterField.Status, true);
            Assert.True(memory.TryRead(address, buffer, 0, 2));
            Assert.Equal(0, BitConverter.ToUInt16(buffer, 0));
        }

        private sealed class TestCharacterLayout : IPlayerCharacterMemoryLayout
        {
            private const long Base = 0x20001000L;

            public long GetAddress(CharacterType character, PlayerCharacterField field, bool forWrite)
            {
                int characterOffset = (int)character * 0x100;

                if (field == PlayerCharacterField.MaxThirst)
                {
                    // Goro is the only character whose max-thirst read uses the
                    // same address as its max-thirst write.
                    if (character == CharacterType.Goro)
                        return Base + characterOffset + (int)PlayerCharacterField.MaxThirst * 0x10 + (forWrite ? 1 : 0);

                    // All other characters read the current thirst value for
                    // GetMaxThirst and write the thirst maximum for SetMaxThirst.
                    if (forWrite)
                        return Base + characterOffset + (int)PlayerCharacterField.MaxThirst * 0x10 + 1;

                    return Base + characterOffset + (int)PlayerCharacterField.Thirst * 0x10;
                }

                int fieldOffset = (int)field * 0x10;
                int writeOffset = forWrite ? 1 : 0;
                return Base + characterOffset + fieldOffset + writeOffset;
            }
        }
    }
}
