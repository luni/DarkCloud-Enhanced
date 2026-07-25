using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Reads game memory and returns the current <see cref="GameSessionState"/>.
    /// Implementations must not sleep or perform I/O beyond reading memory.
    /// </summary>
    public interface IGameSessionDetector
    {
        /// <summary>
        /// Inspects <paramref name="memory"/> and determines the session state.
        /// </summary>
        /// <param name="memory">The memory to inspect, or <c>null</c> if no emulator is connected.</param>
        /// <param name="previousState">The state returned on the previous tick.</param>
        /// <returns>The detected session state.</returns>
        GameSessionState Detect(IGameMemory memory, GameSessionState previousState);
    }
}
