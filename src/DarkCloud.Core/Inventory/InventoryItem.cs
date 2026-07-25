namespace DarkCloud.Core.Inventory
{
    /// <summary>
    /// A single item stack with an identifier and quantity.
    /// </summary>
    public readonly struct InventoryItem
    {
        public InventoryItem(int id, int quantity)
        {
            Id = id;
            Quantity = quantity;
        }

        /// <summary>
        /// The item identifier. Negative values represent empty slots.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The stack quantity.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Returns <c>true</c> when the slot is empty.
        /// </summary>
        public bool IsEmpty => Id <= 0;

        /// <summary>
        /// An empty item slot.
        /// </summary>
        public static InventoryItem Empty => new InventoryItem(-1, 0);
    }
}
