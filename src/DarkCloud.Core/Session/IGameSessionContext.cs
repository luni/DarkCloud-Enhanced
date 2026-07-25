using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Provides context about the current game session to observers.
    /// </summary>
    public interface IGameSessionContext
    {
        /// <summary>
        /// Memory interface available for this session, or <c>null</c> when
        /// no emulator is connected.
        /// </summary>
        IGameMemory Memory { get; }

        /// <summary>
        /// Translator used to map NTSC addresses to the active region.
        /// </summary>
        IAddressTranslator Translator { get; }
    }
}
