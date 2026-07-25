namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Typed writes over an <see cref="IGameMemory"/> backend.
    /// </summary>
    public interface IGameMemoryWriter
    {
        void WriteByte(long address, byte value);
        void WriteUInt16(long address, ushort value);
        void WriteUInt32(long address, uint value);
        void WriteInt32(long address, int value);
        void WriteSingle(long address, float value);
        void WriteString(long address, string value);
    }
}
