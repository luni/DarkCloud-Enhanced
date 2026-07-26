namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Shared constants for the active-item shortcut slots used by
    /// <see cref="ActiveItemService"/> and <see cref="EscapePowderService"/>.
    /// </summary>
    public static class ActiveItemConstants
    {
        public const int EscapePowderItemId = 175;
        public const int RepairPowderItemId = 177;

        /// <summary>
        /// In-memory value that represents an empty active item slot.
        /// The inventory layer treats non-positive IDs as empty, so this must be 0.
        /// </summary>
        public const ushort EmptyItemValue = 0;
    }
}
