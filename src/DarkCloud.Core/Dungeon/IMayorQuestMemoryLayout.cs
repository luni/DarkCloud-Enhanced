namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="MayorQuestService"/>.
    /// </summary>
    public interface IMayorQuestMemoryLayout
    {
        long InDungeonFlagAddress { get; }
        long HideHudAddress { get; }
        long CurrentAllyAddress { get; }
        long ExpectedAllyAddress { get; }
        long AnimationIdAddress { get; }
        long QuestTimerAddress { get; }
        long CompletionAddress { get; }
        long GetEnemyHpAddress(int index);
        int EnemyHpSlotSize { get; }
        int EnemyCount { get; }
    }
}
