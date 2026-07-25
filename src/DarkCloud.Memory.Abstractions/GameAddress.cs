namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// A named NTSC virtual address that can be translated to another region.
    /// </summary>
    public readonly struct GameAddress
    {
        public GameAddress(string name, long ntscAddress, string dataType = null)
        {
            Name = name;
            NtscAddress = ntscAddress;
            DataType = dataType;
        }

        public string Name { get; }
        public long NtscAddress { get; }
        public string DataType { get; }

        /// <summary>
        /// Resolves this address for the given region using the supplied translator.
        /// </summary>
        public long Resolve(IAddressTranslator translator, GameRegion region)
        {
            return translator.Translate(region, NtscAddress);
        }
    }
}
