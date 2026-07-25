namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// A named NTSC virtual address that can be translated to another region.
    /// </summary>
    public readonly struct GameAddress
    {
        public GameAddress(string name, long ntscAddress)
        {
            Name = name;
            NtscAddress = ntscAddress;
        }

        public string Name { get; }
        public long NtscAddress { get; }

        /// <summary>
        /// Resolves this address for the given region using the supplied translator.
        /// </summary>
        public long Resolve(IAddressTranslator translator, GameRegion region)
        {
            return translator.Translate(region, NtscAddress);
        }
    }
}
