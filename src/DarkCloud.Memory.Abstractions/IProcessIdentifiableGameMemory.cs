namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Marks an <see cref="IGameMemory"/> implementation as carrying an emulator
    /// process identity. This lets consumers such as session detectors distinguish
    /// a reconnect to the same process from a switch to a different process.
    /// </summary>
    public interface IProcessIdentifiableGameMemory : IGameMemory
    {
        /// <summary>
        /// The operating-system identifier of the emulator process this memory
        /// interface is bound to.
        /// </summary>
        int ProcessId { get; }
    }
}
