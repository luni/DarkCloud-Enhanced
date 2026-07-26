using DarkCloud.Core.Inventory;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Maps <see cref="InventoryItem"/> reads and writes to the legacy address
    /// constants in <see cref="Player"/> and <see cref="Addresses"/>.
    /// </summary>
    internal sealed class InventoryMemoryLayout : IInventoryMemoryLayout
    {
        public long CurrentSizeAddress => Player.inventoryCurrentSize;

        public long TotalSizeAddress => Player.inventoryTotalSize;

        public long FirstBagItemAddress => Addresses.firstBagItem;

        public long FirstBagWeaponAddress => Addresses.firstBagWeapon;

        public long FirstBagAttachmentAddress => Addresses.firstBagAttachment;

        public int BagItemOffset => 2;

        public int BagWeaponOffset => 0xF8;

        public int BagCharacterWeaponOffset => 0xAA8;

        public int BagAttachmentOffset => 0x20;

        public int BagItemExtraSlotCount => 2;

        public int BagWeaponCount => Player.inventorySizeWeapons;

        public int BagWeaponCountPerCharacter => 10;

        public int BagAttachmentCount => Player.inventorySizeAttachments + 2;

        public int AttachmentCopyLength => 0x1F;

        public long GetActiveItemAddress(int slot)
        {
            switch (slot)
            {
                case 0: return Addresses.activeItem1;
                case 1: return Addresses.activeItem2;
                case 2: return Addresses.activeItem3;
                default: return 0;
            }
        }

        public long GetActiveItemQuantityAddress(int slot)
        {
            switch (slot)
            {
                case 0: return Addresses.activeItem1Quantity;
                case 1: return Addresses.activeItem2Quantity;
                case 2: return Addresses.activeItem3Quantity;
                default: return 0;
            }
        }

        public bool TryGetAttachmentTableSourceAddress(int attachmentId, out long address)
        {
            address = 0;

            if (attachmentId < Items.fire || attachmentId > Items.mageslayer)
                return false;

            address = 0x2027CA60L + ((long)attachmentId - Items.fire) * BagAttachmentOffset;
            return true;
        }
    }
}
