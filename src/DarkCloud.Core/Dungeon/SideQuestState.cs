namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Holds the active side-quest flags derived by <see cref="SideQuestStateService"/>.
    /// </summary>
    public sealed class SideQuestState
    {
        public bool SambaChallengeActive { get; set; }
        public bool MayorQuestActive { get; set; }
    }
}
