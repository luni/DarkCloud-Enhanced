namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Describes the outcome of <see cref="ActiveItemService.Process"/>.
    /// </summary>
    public sealed class ActiveItemResult
    {
        public bool SquareActive { get; set; }
        public bool EscapeConfirmRequested { get; set; }
        public bool EscapeActivated { get; set; }
        public bool RepairPowderUsed { get; set; }
        public bool DunUsedActiveEscape { get; set; }
        public string DisplayMessage { get; set; }
    }
}
