namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Reads and writes the memory-backed state for player characters.
    /// Implementations isolate the PS2 address layout from domain logic.
    /// </summary>
    public interface IPlayerStateRepository
    {
        /// <summary>
        /// Attempts to read a 16-bit unsigned value for the specified character and field.
        /// </summary>
        bool TryReadUInt16(CharacterType character, PlayerCharacterField field, out ushort value);

        /// <summary>
        /// Attempts to read a 32-bit signed value for the specified character and field.
        /// </summary>
        bool TryReadInt32(CharacterType character, PlayerCharacterField field, out int value);

        /// <summary>
        /// Attempts to read a 32-bit floating-point value for the specified character and field.
        /// </summary>
        bool TryReadSingle(CharacterType character, PlayerCharacterField field, out float value);

        /// <summary>
        /// Attempts to read a single byte for the specified character and field.
        /// </summary>
        bool TryReadByte(CharacterType character, PlayerCharacterField field, out byte value);

        /// <summary>
        /// Attempts to write a 16-bit unsigned value for the specified character and field.
        /// </summary>
        bool TryWriteUInt16(CharacterType character, PlayerCharacterField field, ushort value);

        /// <summary>
        /// Attempts to write a 32-bit signed value for the specified character and field.
        /// </summary>
        bool TryWriteInt32(CharacterType character, PlayerCharacterField field, int value);

        /// <summary>
        /// Attempts to write a 32-bit floating-point value for the specified character and field.
        /// </summary>
        bool TryWriteSingle(CharacterType character, PlayerCharacterField field, float value);

        /// <summary>
        /// Attempts to write a single byte for the specified character and field.
        /// </summary>
        bool TryWriteByte(CharacterType character, PlayerCharacterField field, byte value);
    }
}
