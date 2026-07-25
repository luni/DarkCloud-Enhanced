using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Provides the current <see cref="IGameMemory"/> instance and refreshes
    /// the underlying emulator connection when necessary.
    /// </summary>
    public interface IGameMemoryProvider
    {
        /// <summary>
        /// The currently available memory interface, or <c>null</c> if no
        /// emulator is connected.
        /// </summary>
        IGameMemory Current { get; }

        /// <summary>
        /// Attempts to (re)connect to the emulator. Returns <c>true</c> if a
        /// memory interface is available afterwards.
        /// </summary>
        bool TryRefresh();
    }
}
