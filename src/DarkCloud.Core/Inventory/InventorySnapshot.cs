using System;
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
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be non-negative.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");

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
        /// Returns <c>true</c> when the bag is full. A capacity of <c>0</c> is
        /// treated as unknown/uninitialized, so the bag is reported as not full.
        /// </summary>
        public bool IsFull => Capacity > 0 && Count >= Capacity;
    }
}
