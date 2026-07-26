namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// A message to display during a side-quest challenge.
    /// </summary>
    public sealed class SideQuestMessage
    {
        public SideQuestMessage(string text, int height, int width, int displayTime)
        {
            Text = text;
            Height = height;
            Width = width;
            DisplayTime = displayTime;
        }

        public string Text { get; }
        public int Height { get; }
        public int Width { get; }
        public int DisplayTime { get; }
    }
}
