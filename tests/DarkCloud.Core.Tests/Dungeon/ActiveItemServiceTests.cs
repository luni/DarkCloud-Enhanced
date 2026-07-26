using DarkCloud.Core.Dungeon;
using DarkCloud.Memory.Abstractions;
using System;
using Xunit;

namespace DarkCloud.Core.Tests.Dungeon
{
    public class ActiveItemServiceTests
    {
        [Fact]
        public void Process_WhenSquareNotPressed_ReturnsSquareActiveFalse()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            var service = new ActiveItemService(memory, layout);

            var result = service.Process(false, false, false);

            Assert.False(result.SquareActive);
        }

        [Fact]
        public void Process_WhenEscapePowderFirstPress_ReturnsConfirmRequested()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            WriteUShort(memory, 0x1000, ActiveItemService.SquareButton);
            WriteByte(memory, 0x1004, 1);
            WriteInt(memory, 0x1008, -1);
            WriteInt(memory, 0x100C, 1); // slot 1
            WriteShort(memory, 0x1012, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1014, 0);

            var service = new ActiveItemService(memory, layout);
            var result = service.Process(false, false, false);

            Assert.True(result.SquareActive);
            Assert.True(result.EscapeConfirmRequested);
            Assert.NotNull(result.DisplayMessage);
        }

        [Fact]
        public void Process_WhenEscapePowderConfirmed_ActivatesEscape()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            long countAddress = layout.GetPowderCountAddress(1);
            WriteUShort(memory, 0x1000, ActiveItemService.SquareButton);
            WriteByte(memory, 0x1004, 1);
            WriteInt(memory, 0x1008, -1);
            WriteInt(memory, 0x100C, 1);
            WriteShort(memory, 0x1012, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1014, 0);
            WriteByte(memory, 0x1018, 0); // escape flag
            WriteByte(memory, countAddress, 3); // powder count

            var service = new ActiveItemService(memory, layout);
            var result = service.Process(false, true, true);

            Assert.True(result.SquareActive);
            Assert.True(result.EscapeActivated);
            Assert.True(result.DunUsedActiveEscape);
            Assert.Equal(170, ReadByte(memory, 0x1018));
            Assert.Equal(2, ReadByte(memory, countAddress));
        }

        [Fact]
        public void Process_WhenRepairPowderUsed_RepairsWeapon()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            long countAddress = layout.GetPowderCountAddress(1);
            WriteUShort(memory, 0x1000, ActiveItemService.SquareButton);
            WriteByte(memory, 0x1004, 1);
            WriteInt(memory, 0x1008, -1);
            WriteInt(memory, 0x100C, 1);
            WriteShort(memory, 0x1012, ActiveItemConstants.RepairPowderItemId);
            WriteByte(memory, 0x1014, 0);
            WriteUShort(memory, 0x1020, 100); // max whp
            WriteByte(memory, 0x1024, 0); // current char
            WriteByte(memory, 0x1028, 0); // current weapon slot
            WriteFloat(memory, layout.GetCharacterWeaponWhpAddress(0, 0), 50f); // current whp
            WriteByte(memory, countAddress, 2); // powder count

            var service = new ActiveItemService(memory, layout);
            var result = service.Process(false, false, false);

            Assert.True(result.SquareActive);
            Assert.True(result.RepairPowderUsed);
            Assert.Equal(100f, ReadFloat(memory, layout.GetCharacterWeaponWhpAddress(0, 0)));
            Assert.Equal(1, ReadByte(memory, countAddress));
        }

        [Fact]
        public void Process_WhenEscapePowderEmptiesSlot_WritesEmptyItemValue()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            long countAddress = layout.GetPowderCountAddress(1);
            WriteUShort(memory, 0x1000, ActiveItemService.SquareButton);
            WriteByte(memory, 0x1004, 1);
            WriteInt(memory, 0x1008, -1);
            WriteInt(memory, 0x100C, 1);
            WriteShort(memory, 0x1012, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1014, 0);
            WriteByte(memory, 0x1018, 0);
            WriteByte(memory, countAddress, 1);

            var service = new ActiveItemService(memory, layout);
            var result = service.Process(false, true, true);

            Assert.True(result.EscapeActivated);
            Assert.Equal(ActiveItemConstants.EmptyItemValue, ReadUShort(memory, 0x1012));
        }

        [Fact]
        public void Process_WhenCurrentSlotOutOfRange_ReturnsSquareActiveFalse()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x300);
            var layout = CreateLayout(0x1000, 0x1004, 0x1008, 0x100C, 0x1010, 0x1014, 0x1018, 0x101C, 0x1020, 0x1024, 0x1028);
            WriteUShort(memory, 0x1000, ActiveItemService.SquareButton);
            WriteByte(memory, 0x1004, 1);
            WriteInt(memory, 0x1008, -1);
            WriteInt(memory, 0x100C, 5); // invalid slot
            WriteShort(memory, 0x1012, ActiveItemConstants.EscapePowderItemId);
            WriteByte(memory, 0x1014, 0);

            var service = new ActiveItemService(memory, layout);
            var result = service.Process(false, false, false);

            Assert.False(result.SquareActive);
        }

        private static IActiveItemMemoryLayout CreateLayout(long buttons, long usableFlag, long usableInt, long slot, long itemBase, long anim, long escape, long powderCountBase, long maxWhp, long currentChar, long currentWepSlotBase)
        {
            return new FakeActiveItemLayout(buttons, usableFlag, usableInt, slot, itemBase, 2, anim, escape, powderCountBase, maxWhp, currentChar, currentWepSlotBase);
        }

        private static byte ReadByte(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[1];
            Assert.True(memory.TryRead(address, buffer, 0, 1));
            return buffer[0];
        }

        private static short ReadShort(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[2];
            Assert.True(memory.TryRead(address, buffer, 0, 2));
            return BitConverter.ToInt16(buffer, 0);
        }

        private static ushort ReadUShort(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[2];
            Assert.True(memory.TryRead(address, buffer, 0, 2));
            return BitConverter.ToUInt16(buffer, 0);
        }

        private static float ReadFloat(InMemoryGameMemory memory, long address)
        {
            var buffer = new byte[4];
            Assert.True(memory.TryRead(address, buffer, 0, 4));
            return BitConverter.ToSingle(buffer, 0);
        }

        private static void WriteByte(InMemoryGameMemory memory, long address, byte value)
        {
            Assert.True(memory.TryWrite(address, new byte[] { value }, 0, 1));
        }

        private static void WriteShort(InMemoryGameMemory memory, long address, short value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2));
        }

        private static void WriteUShort(InMemoryGameMemory memory, long address, ushort value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 2));
        }

        private static void WriteInt(InMemoryGameMemory memory, long address, int value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4));
        }

        private static void WriteFloat(InMemoryGameMemory memory, long address, float value)
        {
            Assert.True(memory.TryWrite(address, BitConverter.GetBytes(value), 0, 4));
        }

        private sealed class FakeActiveItemLayout : IActiveItemMemoryLayout
        {
            public FakeActiveItemLayout(
                long buttonInputs,
                long usableFlag,
                long usableInt,
                long currentSlot,
                long activeItemBase,
                int activeItemSlotSize,
                long animationId,
                long escapeFlag,
                long powderCountBase,
                long maxWhp,
                long currentChar,
                long currentWeaponSlotBase)
            {
                ButtonInputsAddress = buttonInputs;
                ActiveItemUsableFlagAddress = usableFlag;
                ActiveItemUsableIntAddress = usableInt;
                CurrentSlotAddress = currentSlot;
                ActiveItemBaseAddress = activeItemBase;
                ActiveItemSlotSize = activeItemSlotSize;
                AnimationIdAddress = animationId;
                EscapeFlagAddress = escapeFlag;
                PowderCountBase = powderCountBase;
                CurrentWeaponMaxWhpAddress = maxWhp;
                CurrentCharacterAddress = currentChar;
                CurrentWeaponSlotAddress = currentWeaponSlotBase;
            }

            public long ButtonInputsAddress { get; }
            public long ActiveItemUsableFlagAddress { get; }
            public long ActiveItemUsableIntAddress { get; }
            public long CurrentSlotAddress { get; }
            public long ActiveItemBaseAddress { get; }
            public int ActiveItemSlotSize { get; }
            public int ActiveItemSlotCount => 3;
            public long AnimationIdAddress { get; }
            public long EscapeFlagAddress { get; }
            public long CurrentCharacterAddress { get; }
            public long CurrentWeaponSlotAddress { get; }
            public long CurrentWeaponMaxWhpAddress { get; }
            private long PowderCountBase { get; }

            public long GetPowderCountAddress(int slot) => PowderCountBase + (2L * slot);

            public long GetCharacterWeaponWhpAddress(int character, int weaponSlot)
            {
                return 0x1100 + (character * 0x100L) + (weaponSlot * 0x10L);
            }
        }
    }
}
