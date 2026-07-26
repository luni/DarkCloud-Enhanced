namespace DarkCloud.Core.Dungeon
{
    /// <summary>
    /// Provides the absolute memory addresses used by <see cref="UngagaDoorService"/>.
    /// </summary>
    public interface IUngagaDoorMemoryLayout
    {
        long GetDoorCheckAddress(byte dungeon);
        long GetDoorByte1Address(byte dungeon);
        long GetDoorFloat1Address(byte dungeon);
        long GetDoorFloat2Address(byte dungeon);
        long GetDoorByte2Address(byte dungeon);
        long GetDoorByte3Address(byte dungeon);
    }
}
