using System;
using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class FloorSelectionServiceTests
    {
        [Fact]
        public void Update_WhenCirclePressed_SetsCirclePressed()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeFloorSelectionLayout(0x1020, 0x1030, 0x1040, 0x1050);
            WriteUShort(memory, 0x1020, FloorSelectionService.CircleButton);

            var service = new FloorSelectionService(memory, layout);
            bool pressed = false;
            service.Update(ref pressed, out ushort _);

            Assert.True(pressed);
        }

        [Fact]
        public void Update_WhenCircleReleased_WritesDebugMenuAndMode()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeFloorSelectionLayout(0x1020, 0x1030, 0x1040, 0x1050);
            WriteUShort(memory, 0x1030, 1234);

            var service = new FloorSelectionService(memory, layout);
            bool pressed = true;
            service.Update(ref pressed, out ushort gilda);

            Assert.False(pressed);
            Assert.Equal(1234, gilda);
            Assert.Equal(170, ReadUShort(memory, 0x1040));
            Assert.Equal(1, ReadByte(memory, 0x1050));
        }

        [Fact]
        public void Update_WhenCircleStillHeld_DoesNotWrite()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var layout = new FakeFloorSelectionLayout(0x1020, 0x1030, 0x1040, 0x1050);
            WriteUShort(memory, 0x1020, FloorSelectionService.CircleButton);

            var service = new FloorSelectionService(memory, layout);
            bool pressed = true;
            service.Update(ref pressed, out ushort _);

            Assert.True(pressed);
            Assert.Equal(0, ReadUShort(memory, 0x1040));
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

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2));
        }

        private sealed class FakeFloorSelectionLayout : IFloorSelectionMemoryLayout
        {
            public FakeFloorSelectionLayout(long buttons, long gilda, long debugMenu, long dungeonMode)
            {
                ButtonInputsAddress = buttons;
                GildaAddress = gilda;
                DungeonDebugMenuAddress = debugMenu;
                DungeonModeAddress = dungeonMode;
            }

            public long ButtonInputsAddress { get; }
            public long GildaAddress { get; }
            public long DungeonDebugMenuAddress { get; }
            public long DungeonModeAddress { get; }
        }
    }
}
