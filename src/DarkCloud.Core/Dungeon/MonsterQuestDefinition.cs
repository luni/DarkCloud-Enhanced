namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Describes one monster-hunt side quest for <see cref="MonsterQuestService"/>.
    /// </summary>
    public sealed class MonsterQuestDefinition
    {
        public MonsterQuestDefinition(string name, long targetTypeAddress, long killsRemainingAddress, long completionAddress, byte completionValue, string completionMessage, int displayHeight = 30)
        {
            Name = name;
            TargetTypeAddress = targetTypeAddress;
            KillsRemainingAddress = killsRemainingAddress;
            CompletionAddress = completionAddress;
            CompletionValue = completionValue;
            CompletionMessage = completionMessage;
            DisplayHeight = displayHeight;
        }

        public string Name { get; }
        public long TargetTypeAddress { get; }
        public long KillsRemainingAddress { get; }
        public long CompletionAddress { get; }
        public byte CompletionValue { get; }
        public string CompletionMessage { get; }
        public int DisplayHeight { get; }
    }
}
