using DarkCloud.Core.Weapons;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Weapons
{
    public class WeaponSynthSphereServiceTests
    {
        private const long Level = 0x1000;
        private const long Attack = 0x1002;
        private const long Endurance = 0x1004;
        private const long Speed = 0x1006;
        private const long Magic = 0x1008;
        private const long Slot1ItemId = 0x1010;
        private const long Slot1SynthesisedItemId = 0x1012;
        private const long HasChangedBySynth = 0x1014;
        private const long WeaponFormerStatsValue = 0x1016;

        [Fact]
        public void TryApplyBoost_WithSynthSphere_BoostsToLevelFive()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var addresses = CreateAddresses();
            WriteUShort(memory, Slot1ItemId, 177);
            WriteUShort(memory, Slot1SynthesisedItemId, 0);
            WriteByte(memory, Level, 2);
            WriteUShort(memory, Attack, 10);
            WriteUShort(memory, Endurance, 20);
            WriteUShort(memory, Speed, 30);
            WriteUShort(memory, Magic, 40);
            WriteUShort(memory, HasChangedBySynth, 0);

            var service = new WeaponSynthSphereService(memory);
            bool applied = service.TryApplyBoost(addresses);

            Assert.True(applied);
            Assert.Equal(5, ReadByte(memory, Level));
            Assert.Equal(13, ReadUShort(memory, Attack));
            Assert.Equal(23, ReadUShort(memory, Endurance));
            Assert.Equal(33, ReadUShort(memory, Speed));
            Assert.Equal(43, ReadUShort(memory, Magic));
            Assert.Equal(3, ReadUShort(memory, WeaponFormerStatsValue));
            Assert.Equal(1, ReadUShort(memory, HasChangedBySynth));
        }

        [Fact]
        public void TryApplyBoost_WithoutSynthSphere_DoesNothing()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var addresses = CreateAddresses();
            WriteByte(memory, Level, 2);

            var service = new WeaponSynthSphereService(memory);
            bool applied = service.TryApplyBoost(addresses);

            Assert.False(applied);
            Assert.Equal(2, ReadByte(memory, Level));
        }

        [Fact]
        public void TryApplyBoost_AfterBoost_RevertsWhenSynthSphereRemoved()
        {
            var memory = new InMemoryGameMemory(0x1000, 0x100);
            var addresses = CreateAddresses();
            // Start with the boost applied
            WriteByte(memory, Level, 5);
            WriteUShort(memory, Attack, 13);
            WriteUShort(memory, Endurance, 23);
            WriteUShort(memory, Speed, 33);
            WriteUShort(memory, Magic, 43);
            WriteUShort(memory, HasChangedBySynth, 1);
            WriteUShort(memory, WeaponFormerStatsValue, 3);
            // Slot no longer has an empty synth sphere
            WriteUShort(memory, Slot1ItemId, 0);
            WriteUShort(memory, Slot1SynthesisedItemId, 0);

            var service = new WeaponSynthSphereService(memory);
            bool applied = service.TryApplyBoost(addresses);

            Assert.True(applied);
            Assert.Equal(2, ReadByte(memory, Level));
            Assert.Equal(10, ReadUShort(memory, Attack));
            Assert.Equal(20, ReadUShort(memory, Endurance));
            Assert.Equal(30, ReadUShort(memory, Speed));
            Assert.Equal(40, ReadUShort(memory, Magic));
            Assert.Equal(0, ReadUShort(memory, WeaponFormerStatsValue));
            Assert.Equal(0, ReadUShort(memory, HasChangedBySynth));
        }

        private static WeaponSlotAddresses CreateAddresses()
        {
            return new WeaponSlotAddresses(
                Level,
                Attack,
                Endurance,
                Speed,
                Magic,
                Slot1ItemId,
                Slot1SynthesisedItemId,
                HasChangedBySynth,
                WeaponFormerStatsValue);
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
    }
}
