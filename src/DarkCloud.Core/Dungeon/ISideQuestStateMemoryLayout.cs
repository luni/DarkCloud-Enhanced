namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="SideQuestStateService"/>.
    /// </summary>
    public interface ISideQuestStateMemoryLayout
    {
        long SambaChallengeFlagAddress { get; }
        long MayorQuestFlagAddress { get; }
        long MayorQuestFloorAddress { get; }
    }
}
