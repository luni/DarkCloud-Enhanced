using DarkCloud.Memory.Abstractions;

namespace DarkCloud.Core.Session
{
    /// <summary>
    /// Address translator that returns the requested address unchanged. Useful
    /// for tests and for NTSC-only scenarios where no translation is required.
    /// </summary>
    public sealed class PassthroughAddressTranslator : IAddressTranslator
    {
        public long Translate(GameRegion region, long address)
        {
            return address;
        }
    }
}
