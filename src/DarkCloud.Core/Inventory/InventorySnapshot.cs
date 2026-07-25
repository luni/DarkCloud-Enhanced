using System.Collections.Generic;

namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// A read-only view of the current inventory state.
    /// </summary>
    public sealed class InventorySnapshot
    {
        public InventorySnapshot(int capacity, int count, IReadOnlyList<InventoryItem> activeItems)
        {
            Capacity = capacity;
            Count = count;
            ActiveItems = activeItems ?? new InventoryItem[0];
        }

        /// <summary>
        /// The maximum number of bag slots.
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// The number of bag slots currently occupied.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The three active item slots.
        /// </summary>
        public IReadOnlyList<InventoryItem> ActiveItems { get; }

        /// <summary>
        /// Returns <c>true</c> when the bag is full.
        /// </summary>
        public bool IsFull => Count >= Capacity;
    }
}
