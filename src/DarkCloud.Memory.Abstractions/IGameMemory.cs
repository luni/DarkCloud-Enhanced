namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Low-level contract for reading and writing bytes from a game memory backend.
    /// All byte I/O uses the host platform's byte order (little-endian on supported
    /// Windows and Linux builds).
    /// </summary>
    public interface IGameMemory
    {
        /// <summary>
        /// Reads up to <paramref name="count"/> bytes into <paramref name="destination"/>
        /// starting at <paramref name="offset"/>.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="destination">The buffer to receive the bytes.</param>
        /// <param name="offset">The zero-based index in <paramref name="destination"/> to start writing.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>
        /// <c>true</c> if all requested bytes were read; otherwise <c>false</c>.
        /// </returns>
        bool TryRead(long address, byte[] destination, int offset, int count);

        /// <summary>
        /// Writes up to <paramref name="count"/> bytes from <paramref name="source"/>
        /// starting at <paramref name="offset"/>.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="source">The buffer containing the bytes to write.</param>
        /// <param name="offset">The zero-based index in <paramref name="source"/> to start reading.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns>
        /// <c>true</c> if all requested bytes were written; otherwise <c>false</c>.
        /// </returns>
        bool TryWrite(long address, byte[] source, int offset, int count);
    }
}
