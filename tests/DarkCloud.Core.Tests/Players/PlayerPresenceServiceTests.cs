using DarkCloud.Core.Players;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Players
{
    public class PlayerPresenceServiceTests
    {
        [Theory]
        [InlineData(0, CharacterType.Toan)]
        [InlineData(1, CharacterType.Xiao)]
        [InlineData(2, CharacterType.Goro)]
        [InlineData(3, CharacterType.Ruby)]
        [InlineData(4, CharacterType.Ungaga)]
        [InlineData(5, CharacterType.Osmond)]
        [InlineData(255, CharacterType.Unknown)]
        [InlineData(99, CharacterType.Unknown)]
        public void GetCurrentCharacter_MapsMemoryValueToCharacterType(byte value, CharacterType expected)
        {
            var memory = new InMemoryGameMemory();
            memory.Load(new byte[] { value }, (int)(0x21CD9550L - InMemoryGameMemory.DefaultBaseAddress));

            var service = new PlayerPresenceService(memory);

            Assert.Equal(expected, service.GetCurrentCharacter());
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(100, true)]
        [InlineData(255, false)]
        public void IsInDungeonFloor_ReturnsTrueForAnyValueExcept255(byte value, bool expected)
        {
            var memory = new InMemoryGameMemory();
            memory.Load(new byte[] { value }, (int)(0x21CD954FL - InMemoryGameMemory.DefaultBaseAddress));

            var service = new PlayerPresenceService(memory);

            Assert.Equal(expected, service.IsInDungeonFloor());
        }

        [Fact]
        public void GetCurrentCharacter_WhenReadFails_ReturnsUnknown()
        {
            // A tiny memory buffer forces the address read to fail.
            var memory = new InMemoryGameMemory(InMemoryGameMemory.DefaultBaseAddress, 1024);
            var service = new PlayerPresenceService(memory);

            Assert.Equal(CharacterType.Unknown, service.GetCurrentCharacter());
        }

        [Fact]
        public void IsInDungeonFloor_WhenReadFails_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory(InMemoryGameMemory.DefaultBaseAddress, 1024);
            var service = new PlayerPresenceService(memory);

            Assert.False(service.IsInDungeonFloor());
        }

        [Theory]
        [InlineData(CharacterType.Toan, "Toan")]
        [InlineData(CharacterType.Xiao, "Xiao")]
        [InlineData(CharacterType.Goro, "Goro")]
        [InlineData(CharacterType.Ruby, "Ruby")]
        [InlineData(CharacterType.Ungaga, "Ungaga")]
        [InlineData(CharacterType.Osmond, "Osmond")]
        [InlineData(CharacterType.Unknown, null)]
        public void GetName_ReturnsDisplayNameOrNull(CharacterType character, string expected)
        {
            Assert.Equal(expected, character.GetName());
        }

        [Fact]
        public void GetName_ForUnrecognizedCastValue_ReturnsNull()
        {
            Assert.Null(((CharacterType)42).GetName());
        }
    }
}
