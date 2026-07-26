using System.Collections.Generic;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// The outcome of <see cref="MonsterQuestService.Process"/>.
    /// </summary>
    public sealed class MonsterQuestResult
    {
        public MonsterQuestResult(IReadOnlyList<int> progressed, IReadOnlyList<int> completed)
        {
            ProgressedQuestIndices = progressed;
            CompletedQuestIndices = completed;
        }

        public IReadOnlyList<int> ProgressedQuestIndices { get; }
        public IReadOnlyList<int> CompletedQuestIndices { get; }
    }
}
