using System.Collections.Generic;

namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// Reads and writes the memory-backed inventory state.
    /// Implementations isolate the PS2 address layout from domain logic.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Reads a snapshot of the current active-item and bag-capacity state.
        /// </summary>
        InventorySnapshot Read();

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
        /// Writes an item into one of the three active item slots.
        /// </summary>
        /// <param name="slot">The active item slot index (0, 1, or 2).</param>
        /// <param name="item">The item to write.</param>
        /// <returns><c>true</c> if the write succeeded; otherwise, <c>false</c>.</returns>
        bool TryWriteActiveItem(int slot, InventoryItem item);

        /// <summary>
        /// Writes an item ID into the given item bag slot.
        /// </summary>
        /// <param name="slot">The bag slot index.</param>
        /// <param name="item">The item to write.</param>
        /// <returns><c>true</c> if the write succeeded; otherwise, <c>false</c>.</returns>
        bool TryWriteBagItem(int slot, InventoryItem item);

        /// <summary>
        /// Copies an attachment's template data into the given attachment bag slot.
        /// </summary>
        /// <param name="slot">The attachment bag slot index.</param>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <returns><c>true</c> if the copy succeeded; otherwise, <c>false</c>.</returns>
        bool TryCopyAttachment(int slot, int attachmentId);
    }
}
