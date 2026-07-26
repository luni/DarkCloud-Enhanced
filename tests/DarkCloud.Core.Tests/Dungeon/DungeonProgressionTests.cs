using System.Linq;
using DarkCloud.Core.Dungeon;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class DungeonProgressionTests
    {
        [Theory]
        [InlineData(0, new byte[] { 195 })]
        [InlineData(1, new byte[] { 196, 198, 205 })]
        [InlineData(2, new byte[] { 201 })]
        [InlineData(3, new byte[] { 202 })]
        [InlineData(4, new byte[] { 203 })]
        [InlineData(5, new byte[] { 204 })]
        [InlineData(6, new byte[] { 206 })]
        [InlineData(255, new byte[0])]
        public void GetGateKeyItems_ReturnsExpectedItems(byte dungeon, byte[] expected)
        {
            Assert.Equal(expected, DungeonProgression.GetGateKeyItems(dungeon));
        }

        [Theory]
        [InlineData(0, 224)]
        [InlineData(1, 225)]
        [InlineData(2, 226)]
        [InlineData(3, 228)]
        [InlineData(4, 229)]
        [InlineData(5, 230)]
        [InlineData(6, 231)]
        [InlineData(255, 255)]
        public void GetBackFloorKeyItem_ReturnsExpectedItem(byte dungeon, byte expected)
        {
            Assert.Equal(expected, DungeonProgression.GetBackFloorKeyItem(dungeon));
        }

        [Theory]
        [InlineData(0, new byte[] { 3, 7, 14 })]
        [InlineData(1, new byte[] { 8, 16 })]
        [InlineData(2, new byte[] { 8, 17 })]
        [InlineData(3, new byte[] { 8, 17 })]
        [InlineData(4, new byte[] { 7, 14 })]
        [InlineData(5, new byte[] { 24 })]
        [InlineData(6, new byte[] { 99 })]
        [InlineData(255, new byte[0])]
        public void GetEventFloors_ReturnsExpectedFloors(byte dungeon, byte[] expected)
        {
            Assert.Equal(expected, DungeonProgression.GetEventFloors(dungeon));
        }
    }
}
