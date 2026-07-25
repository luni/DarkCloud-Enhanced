using System;
using System.Collections.Generic;
using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// Memory-backed implementation of <see cref="IInventoryRepository"/>.
    /// </summary>
    public sealed class InventoryRepository : IInventoryRepository
    {
        private readonly IGameMemory _memory;
        private readonly IInventoryMemoryLayout _layout;

        public InventoryRepository(IGameMemory memory, IInventoryMemoryLayout layout)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        }

        public InventorySnapshot Read()
        {
            int capacity = ReadByte(_layout.TotalSizeAddress);
            int count = ReadByte(_layout.CurrentSizeAddress);

            var activeItems = new InventoryItem[3];
            for (int slot = 0; slot < 3; slot++)
            {
                activeItems[slot] = ReadActiveItem(slot);
            }

            return new InventorySnapshot(capacity, count, activeItems);
        }

        public IReadOnlyList<InventoryItem> ReadBagItems()
        {
            int capacity = ReadByte(_layout.TotalSizeAddress);
            int count = capacity + _layout.BagItemExtraSlotCount;

            var items = new InventoryItem[count];
            for (int slot = 0; slot < count; slot++)
            {
                long address = _layout.FirstBagItemAddress + (long)slot * _layout.BagItemOffset;
                int id = ReadUInt16(address);
                items[slot] = AsInventoryItem(id, quantity: 1);
            }

            return items;
        }

        public IReadOnlyList<InventoryItem> ReadBagWeapons(int character = -1)
        {
            int count;
            long baseAddress;

            if (character == -1)
            {
                count = _layout.BagWeaponCount;
                baseAddress = _layout.FirstBagWeaponAddress;
            }
            else if (character >= 0 && character <= 5)
            {
                count = _layout.BagWeaponCountPerCharacter;
                baseAddress = _layout.FirstBagWeaponAddress + (long)character * _layout.BagCharacterWeaponOffset;
            }
            else
            {
                return Array.Empty<InventoryItem>();
            }

            var items = new InventoryItem[count];
            for (int slot = 0; slot < count; slot++)
            {
                long address = baseAddress + (long)slot * _layout.BagWeaponOffset;
                int id = ReadUInt16(address);
                items[slot] = AsInventoryItem(id, quantity: 1);
            }

            return items;
        }

        public IReadOnlyList<InventoryItem> ReadBagAttachments()
        {
            int count = _layout.BagAttachmentCount;
            var items = new InventoryItem[count];

            for (int slot = 0; slot < count; slot++)
            {
                long address = _layout.FirstBagAttachmentAddress + (long)slot * _layout.BagAttachmentOffset;
                int id = ReadUInt16(address);
                items[slot] = AsInventoryItem(id, quantity: 1);
            }

            return items;
        }

        public bool TryWriteActiveItem(int slot, InventoryItem item)
        {
            if (slot < 0 || slot > 2)
                return false;

            if (item.Id < 0 || item.Id > ushort.MaxValue || item.Quantity < 0 || item.Quantity > ushort.MaxValue)
                return false;

            long idAddress = _layout.GetActiveItemAddress(slot);
            long quantityAddress = _layout.GetActiveItemQuantityAddress(slot);

            byte[] idBytes = BitConverter.GetBytes((ushort)item.Id);
            byte[] quantityBytes = BitConverter.GetBytes((ushort)item.Quantity);

            if (!_memory.TryWrite(idAddress, idBytes, 0, idBytes.Length))
                return false;

            return _memory.TryWrite(quantityAddress, quantityBytes, 0, quantityBytes.Length);
        }

        public bool TryWriteBagItem(int slot, InventoryItem item)
        {
            int capacity = ReadByte(_layout.TotalSizeAddress);
            int maxSlot = capacity + _layout.BagItemExtraSlotCount;
            if (slot < 0 || slot >= maxSlot)
                return false;

            if (item.Id < 0 || item.Id > ushort.MaxValue)
                return false;

            long address = _layout.FirstBagItemAddress + (long)slot * _layout.BagItemOffset;
            byte[] idBytes = BitConverter.GetBytes((ushort)item.Id);
            return _memory.TryWrite(address, idBytes, 0, idBytes.Length);
        }

        public bool TryCopyAttachment(int slot, int attachmentId)
        {
            if (slot < 0 || slot >= _layout.BagAttachmentCount)
                return false;

            if (!_layout.TryGetAttachmentTableSourceAddress(attachmentId, out long sourceAddress))
                return false;

            int length = _layout.AttachmentCopyLength;
            byte[] buffer = new byte[length];
            if (!_memory.TryRead(sourceAddress, buffer, 0, length))
                return false;

            long destinationAddress = _layout.FirstBagAttachmentAddress + (long)slot * _layout.BagAttachmentOffset;
            return _memory.TryWrite(destinationAddress, buffer, 0, length);
        }

        private InventoryItem ReadActiveItem(int slot)
        {
            long idAddress = _layout.GetActiveItemAddress(slot);
            long quantityAddress = _layout.GetActiveItemQuantityAddress(slot);

            int id = ReadUInt16(idAddress);
            int quantity = ReadUInt16(quantityAddress);

            return AsInventoryItem(id, quantity);
        }

        private InventoryItem AsInventoryItem(int id, int quantity)
        {
            if (id <= 0)
                return InventoryItem.Empty;

            return new InventoryItem(id, quantity);
        }

        private byte ReadByte(long address)
        {
            byte[] buffer = new byte[1];
            if (!_memory.TryRead(address, buffer, 0, 1))
                return 0;

            return buffer[0];
        }

        private ushort ReadUInt16(long address)
        {
            byte[] buffer = new byte[2];
            if (!_memory.TryRead(address, buffer, 0, 2))
                return 0;

            return BitConverter.ToUInt16(buffer, 0);
        }
    }
}
