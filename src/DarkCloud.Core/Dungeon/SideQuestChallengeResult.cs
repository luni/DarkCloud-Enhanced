using System.Collections.Generic;

namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// The outcome of a side-quest challenge tick.
    /// </summary>
    public sealed class SideQuestChallengeResult
    {
        public SideQuestChallengeResult()
        {
            Messages = new List<SideQuestMessage>();
        }

        public bool QuestCheck { get; set; }
        public bool QuestActive { get; set; }
        public bool Quest { get; set; }
        public bool[] MonstersDead { get; set; }
        public IList<SideQuestMessage> Messages { get; }
        public bool ShouldDelayFirstMessage { get; set; }
    }
}
