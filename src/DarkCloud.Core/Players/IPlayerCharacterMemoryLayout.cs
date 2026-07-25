namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Provides the PS2 memory address for a given character and field.
    /// Implementations live in the host application and map the legacy address
    /// constants to the core abstraction.
    /// </summary>
    public interface IPlayerCharacterMemoryLayout
    {
        /// <summary>
        /// Returns the address for the specified character field.
        /// </summary>
        /// <param name="character">The character whose memory is being accessed.</param>
        /// <param name="field">The field to access.</param>
        /// <param name="forWrite">
        /// <c>true</c> when the caller intends to write; some fields have
        /// historically used different addresses for reads and writes.
        /// </param>
        long GetAddress(CharacterType character, PlayerCharacterField field, bool forWrite);
    }
}
