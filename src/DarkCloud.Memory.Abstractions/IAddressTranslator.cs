namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Translates an NTSC virtual address to the equivalent address in another
    /// console region.
    /// </summary>
    public interface IAddressTranslator
    {
        /// <summary>
        /// Translates <paramref name="ntscAddress"/> for the given <paramref name="region"/>.
        /// </summary>
        /// <param name="region">The target region.</param>
        /// <param name="ntscAddress">The address in the NTSC layout.</param>
        /// <returns>The translated address, or the original address when translation is not required.</returns>
        long Translate(GameRegion region, long ntscAddress);
    }
}
