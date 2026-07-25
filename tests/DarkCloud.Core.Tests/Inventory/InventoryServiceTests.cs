using System;
using System.Linq;
using DarkCloud.Core.Inventory;
using DarkCloud.Memory.Abstractions;
using Xunit;

namespace DarkCloud.Core.Tests.Inventory
{
    public class InventoryServiceTests
    {
        private static InventoryService CreateService(IGameMemory memory)
        {
            var layout = new TestInventoryLayout();
            var repository = new InventoryRepository(memory, layout);
            return new InventoryService(repository);
        }

        [Fact]
        public void ReadSnapshot_ReturnsCapacityAndActiveItems()
        {
            var memory = new InMemoryGameMemory();
            byte capacity = 20;
            byte count = 5;
            ushort item1 = 150;
            ushort quantity1 = 3;

            var layout = new TestInventoryLayout();
            memory.Load(new[] { capacity }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(new[] { count }, (int)(layout.CurrentSizeAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(BitConverter.GetBytes(item1), (int)(layout.GetActiveItemAddress(0) - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(BitConverter.GetBytes(quantity1), (int)(layout.GetActiveItemQuantityAddress(0) - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);
            var snapshot = service.ReadSnapshot();

            Assert.Equal(capacity, snapshot.Capacity);
            Assert.Equal(count, snapshot.Count);
            Assert.False(snapshot.IsFull);
            Assert.Equal(item1, snapshot.ActiveItems[0].Id);
            Assert.Equal(quantity1, snapshot.ActiveItems[0].Quantity);
        }

        [Fact]
        public void IsBagFull_ReturnsTrueWhenCountReachesCapacity()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            memory.Load(new[] { (byte)10 }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(new[] { (byte)10 }, (int)(layout.CurrentSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.True(service.IsBagFull());
        }

        [Fact]
        public void TrySetActiveItem_WhenBagFull_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            memory.Load(new[] { (byte)10 }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(new[] { (byte)10 }, (int)(layout.CurrentSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.False(service.TrySetActiveItem(0, 150, 3));
        }

        [Fact]
        public void TrySetActiveItem_WithValidSlotAndSpace_WritesToMemory()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            memory.Load(new[] { (byte)10 }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(new[] { (byte)5 }, (int)(layout.CurrentSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.True(service.TrySetActiveItem(1, 200, 7));

            byte[] idBuffer = new byte[2];
            byte[] quantityBuffer = new byte[2];
            Assert.True(memory.TryRead(layout.GetActiveItemAddress(1), idBuffer, 0, 2));
            Assert.True(memory.TryRead(layout.GetActiveItemQuantityAddress(1), quantityBuffer, 0, 2));
            Assert.Equal(200, BitConverter.ToUInt16(idBuffer, 0));
            Assert.Equal(7, BitConverter.ToUInt16(quantityBuffer, 0));
        }

        [Theory]
        [InlineData(-1, 150, 3)]
        [InlineData(3, 150, 3)]
        [InlineData(0, -1, 3)]
        [InlineData(0, 70000, 3)]
        [InlineData(0, 150, -1)]
        [InlineData(0, 150, 70000)]
        public void TrySetActiveItem_InvalidArguments_ReturnsFalse(int slot, int itemId, int quantity)
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            Assert.False(service.TrySetActiveItem(slot, itemId, quantity));
        }

        [Fact]
        public void ReadBagItems_ReturnsItemBagContents()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            byte capacity = 3;
            memory.Load(new[] { capacity }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            ushort item1 = 150;
            ushort item2 = 200;
            memory.Load(BitConverter.GetBytes(item1), (int)(layout.FirstBagItemAddress - InMemoryGameMemory.DefaultBaseAddress));
            memory.Load(BitConverter.GetBytes(item2), (int)(layout.FirstBagItemAddress + layout.BagItemOffset - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);
            var bag = service.ReadBagItems();

            Assert.Equal(capacity + layout.BagItemExtraSlotCount, bag.Count);
            Assert.Equal(item1, bag[0].Id);
            Assert.Equal(item2, bag[1].Id);
            Assert.True(bag[2].IsEmpty);
            Assert.True(bag[3].IsEmpty);
            Assert.True(bag[4].IsEmpty);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ReadBagWeapons_PerCharacter_ReturnsWeapons(int character)
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            ushort weaponId = 300;
            long address = layout.FirstBagWeaponAddress + (long)character * layout.BagCharacterWeaponOffset;
            memory.Load(BitConverter.GetBytes(weaponId), (int)(address - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);
            var weapons = service.ReadBagWeapons(character);

            Assert.Equal(layout.BagWeaponCountPerCharacter, weapons.Count);
            Assert.Equal(weaponId, weapons[0].Id);
            Assert.True(weapons.Skip(1).All(w => w.IsEmpty));
        }

        [Fact]
        public void ReadBagWeapons_AllCharacters_ReturnsAllSlots()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();

            var service = CreateService(memory);
            var weapons = service.ReadBagWeapons(-1);

            Assert.Equal(layout.BagWeaponCount, weapons.Count);
        }

        [Fact]
        public void ReadBagAttachments_ReturnsAttachmentBagContents()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            ushort attachmentId = 90;
            memory.Load(BitConverter.GetBytes(attachmentId), (int)(layout.FirstBagAttachmentAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);
            var attachments = service.ReadBagAttachments();

            Assert.Equal(layout.BagAttachmentCount, attachments.Count);
            Assert.Equal(attachmentId, attachments[0].Id);
            Assert.True(attachments.Skip(1).All(a => a.IsEmpty));
        }

        [Fact]
        public void TrySetBagItem_WithValidSlot_WritesToMemory()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            byte capacity = 5;
            memory.Load(new[] { capacity }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.True(service.TrySetBagItem(2, 150));

            byte[] buffer = new byte[2];
            Assert.True(memory.TryRead(layout.FirstBagItemAddress + 2 * layout.BagItemOffset, buffer, 0, 2));
            Assert.Equal(150, BitConverter.ToUInt16(buffer, 0));
        }

        [Theory]
        [InlineData(-1, 150)]
        [InlineData(0, 70000)]
        [InlineData(100, 150)]
        public void TrySetBagItem_InvalidArguments_ReturnsFalse(int slot, int itemId)
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            memory.Load(new[] { (byte)5 }, (int)(layout.TotalSizeAddress - InMemoryGameMemory.DefaultBaseAddress));

            var service = CreateService(memory);

            Assert.False(service.TrySetBagItem(slot, itemId));
        }

        [Fact]
        public void TrySetBagAttachment_CopiesTemplateBytesToBag()
        {
            var memory = new InMemoryGameMemory();
            var layout = new TestInventoryLayout();
            var service = CreateService(memory);

            int attachmentId = 81;
            byte[] template = Enumerable.Range(1, layout.AttachmentCopyLength).Select(i => (byte)i).ToArray();
            memory.Load(template, (int)(layout.GetAttachmentTableSourceAddress(attachmentId).GetValueOrDefault() - InMemoryGameMemory.DefaultBaseAddress));

            Assert.True(service.TrySetBagAttachment(0, attachmentId));

            byte[] destination = new byte[layout.AttachmentCopyLength];
            Assert.True(memory.TryRead(layout.FirstBagAttachmentAddress, destination, 0, destination.Length));
            Assert.Equal(template, destination);
        }

        [Fact]
        public void TrySetBagAttachment_WithInvalidAttachmentId_ReturnsFalse()
        {
            var memory = new InMemoryGameMemory();
            var service = CreateService(memory);

            Assert.False(service.TrySetBagAttachment(0, 9999));
            Assert.False(service.TrySetBagAttachment(0, -1));
        }

        private sealed class TestInventoryLayout : IInventoryMemoryLayout
        {
            private const long Base = 0x20002000L;

            public long CurrentSizeAddress => Base;

            public long TotalSizeAddress => Base + 1;

            public long FirstBagItemAddress => Base + 0x100;

            public long FirstBagWeaponAddress => Base + 0x200;

            public long FirstBagAttachmentAddress => Base + 0x300;

            public int BagItemOffset => 2;

            public int BagWeaponOffset => 0xF8;

            public int BagCharacterWeaponOffset => 0xAA8;

            public int BagAttachmentOffset => 0x20;

            public int BagItemExtraSlotCount => 2;

            public int BagWeaponCount => 65;

            public int BagWeaponCountPerCharacter => 10;

            public int BagAttachmentCount => 42;

            public int AttachmentCopyLength => 0x1F;

            public long GetActiveItemAddress(int slot)
            {
                return Base + 0x10 + slot * 0x4;
            }

            public long GetActiveItemQuantityAddress(int slot)
            {
                return Base + 0x12 + slot * 0x4;
            }

            public long? GetAttachmentTableSourceAddress(int attachmentId)
            {
                if (attachmentId < 81 || attachmentId > 120)
                    return null;

                return Base + 0x1000 + (long)(attachmentId - 81) * BagAttachmentOffset;
            }

            public bool TryGetAttachmentTableSourceAddress(int attachmentId, out long address)
            {
                long? value = GetAttachmentTableSourceAddress(attachmentId);
                address = value ?? 0;
                return value.HasValue;
            }
        }
    }
}
