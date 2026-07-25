namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// Provides the PS2 memory addresses used to read and write inventory state.
    /// </summary>
    public interface IInventoryMemoryLayout
    {
        /// <summary>
        /// Address of the byte holding the current number of occupied bag slots.
        /// </summary>
        long CurrentSizeAddress { get; }

        /// <summary>
        /// Address of the byte holding the maximum number of bag slots.
        /// </summary>
        long TotalSizeAddress { get; }

        /// <summary>
        /// Returns the address for the active item ID in the given slot (0-2).
        /// </summary>
        long GetActiveItemAddress(int slot);

        /// <summary>
        /// Returns the address for the active item quantity in the given slot (0-2).
        /// </summary>
        long GetActiveItemQuantityAddress(int slot);

        /// <summary>
        /// Address of the first item bag slot.
        /// </summary>
        long FirstBagItemAddress { get; }

        /// <summary>
        /// Address of the first weapon bag slot.
        /// </summary>
        long FirstBagWeaponAddress { get; }

        /// <summary>
        /// Address of the first attachment bag slot.
        /// </summary>
        long FirstBagAttachmentAddress { get; }

        /// <summary>
        /// Offset, in bytes, between two item bag slots.
        /// </summary>
        int BagItemOffset { get; }

        /// <summary>
        /// Offset, in bytes, between two weapon bag slots for the same character.
        /// </summary>
        int BagWeaponOffset { get; }

        /// <summary>
        /// Offset, in bytes, between the first weapon slot of two consecutive
        /// characters.
        /// </summary>
        int BagCharacterWeaponOffset { get; }

        /// <summary>
        /// Offset, in bytes, between two attachment bag slots.
        /// </summary>
        int BagAttachmentOffset { get; }

        /// <summary>
        /// Number of extra item bag slots reserved past the current capacity.
        /// </summary>
        int BagItemExtraSlotCount { get; }

        /// <summary>
        /// Total number of weapon bag slots across all characters.
        /// </summary>
        int BagWeaponCount { get; }

        /// <summary>
        /// Number of weapon bag slots per character.
        /// </summary>
        int BagWeaponCountPerCharacter { get; }

        /// <summary>
        /// Total number of attachment bag slots.
        /// </summary>
        int BagAttachmentCount { get; }

        /// <summary>
        /// Number of bytes to copy when placing an attachment in a bag slot.
        /// </summary>
        int AttachmentCopyLength { get; }

        /// <summary>
        /// Attempts to resolve the source address for an attachment's template
        /// data in the attachment table.
        /// </summary>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <param name="address">The source address when valid.</param>
        /// <returns><c>true</c> when the attachment identifier maps to a table entry; otherwise, <c>false</c>.</returns>
        bool TryGetAttachmentTableSourceAddress(int attachmentId, out long address);
    }
}
