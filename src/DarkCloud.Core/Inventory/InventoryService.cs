using System;
using System.Collections.Generic;

namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// Default implementation of <see cref="IInventoryService"/>.
    /// </summary>
    public sealed class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;

        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public InventorySnapshot ReadSnapshot()
        {
            return _repository.Read();
        }

        public bool IsBagFull()
        {
            InventorySnapshot snapshot = _repository.Read();
            return snapshot.IsFull;
        }

        public bool TrySetActiveItem(int slot, int itemId, int quantity)
        {
            if (slot < 0 || slot > 2)
                return false;

            if (itemId < 0 || itemId > ushort.MaxValue)
                return false;

            if (quantity < 0 || quantity > ushort.MaxValue)
                return false;

            InventorySnapshot snapshot = _repository.Read();
            if (snapshot.IsFull)
                return false;

            return _repository.TryWriteActiveItem(slot, new InventoryItem(itemId, quantity));
        }

        public IReadOnlyList<InventoryItem> ReadBagItems()
        {
            return _repository.ReadBagItems();
        }

        public IReadOnlyList<InventoryItem> ReadBagWeapons(int character = -1)
        {
            return _repository.ReadBagWeapons(character);
        }

        public IReadOnlyList<InventoryItem> ReadBagAttachments()
        {
            return _repository.ReadBagAttachments();
        }

        public bool TrySetBagItem(int slot, int itemId)
        {
            if (slot < 0)
                return false;

            if (itemId < 0 || itemId > ushort.MaxValue)
                return false;

            return _repository.TryWriteBagItem(slot, new InventoryItem(itemId, quantity: 1));
        }

        public bool TrySetBagAttachment(int slot, int attachmentId)
        {
            if (slot < 0)
                return false;

            if (attachmentId < 0 || attachmentId > ushort.MaxValue)
                return false;

            return _repository.TryCopyAttachment(slot, attachmentId);
        }
    }
}
