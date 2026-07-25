namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Typed reads over an <see cref="IGameMemory"/> backend.
    /// </summary>
    public interface IGameMemoryReader
    {
        byte ReadByte(long address);
        ushort ReadUInt16(long address);
        uint ReadUInt32(long address);
        int ReadInt32(long address);
        float ReadSingle(long address);
        string ReadString(long address, int length);
    }
}
