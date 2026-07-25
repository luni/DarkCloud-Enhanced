using System.Collections.Generic;

namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// Domain service for inventory rules such as capacity validation and
    /// active-item management.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Reads a snapshot of the current active-item and bag-capacity state.
        /// </summary>
        InventorySnapshot ReadSnapshot();

        /// <summary>
        /// Returns <c>true</c> when the bag is full.
        /// </summary>
        bool IsBagFull();

        /// <summary>
        /// Attempts to set an active item slot. The bag must have free capacity.
        /// </summary>
        /// <param name="slot">The active item slot index (0, 1, or 2).</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="quantity">The item quantity.</param>
        /// <returns><c>true</c> if the item was set; otherwise, <c>false</c>.</returns>
        bool TrySetActiveItem(int slot, int itemId, int quantity);

        /// <summary>
        /// Reads the current item bag contents.
        /// </summary>
        IReadOnlyList<InventoryItem> ReadBagItems();

        /// <summary>
        /// Reads the current weapon bag contents.
        /// </summary>
        /// <param name="character">Character index (0-5), or -1 for all 65 bag slots.</param>
        IReadOnlyList<InventoryItem> ReadBagWeapons(int character = -1);

        /// <summary>
        /// Reads the current attachment bag contents.
        /// </summary>
        IReadOnlyList<InventoryItem> ReadBagAttachments();

        /// <summary>
        /// Writes an item ID into the given item bag slot.
        /// </summary>
        /// <param name="slot">The bag slot index.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <returns><c>true</c> if the write succeeded; otherwise, <c>false</c>.</returns>
        bool TrySetBagItem(int slot, int itemId);

        /// <summary>
        /// Copies an attachment's template data into the given attachment bag slot.
        /// </summary>
        /// <param name="slot">The attachment bag slot index.</param>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <returns><c>true</c> if the copy succeeded; otherwise, <c>false</c>.</returns>
        bool TrySetBagAttachment(int slot, int attachmentId);
    }
}
