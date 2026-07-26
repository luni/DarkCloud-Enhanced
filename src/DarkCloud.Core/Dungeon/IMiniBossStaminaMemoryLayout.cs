namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the memory addresses used by <see cref="MiniBossStaminaService"/>.
    /// </summary>
    public interface IMiniBossStaminaMemoryLayout
    {
        long GetStaminaTimerAddress(int enemyNumber);
        long BackFloorFlagAddress { get; }
    }
}
