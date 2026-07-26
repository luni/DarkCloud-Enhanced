using System.Collections.Generic;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="SambaChallengeService"/>.
    /// </summary>
    public interface ISambaChallengeMemoryLayout
    {
        long CurrentWeaponIdAddress { get; }
        long InDungeonFlagAddress { get; }
        long HideHudAddress { get; }
        long CurrentAllyAddress { get; }
        long AnimationIdAddress { get; }
        long QuestTimerAddress { get; }
        long CompletionAddress { get; }
        IReadOnlyList<ushort> AllowedWeaponIds { get; }
        long GetEnemyHpAddress(int index);
        int EnemyHpSlotSize { get; }
        int EnemyCount { get; }
    }
}
