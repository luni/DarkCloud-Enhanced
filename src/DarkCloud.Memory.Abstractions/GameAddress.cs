namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// A named NTSC virtual address that may have an explicit PAL equivalent and
    /// can be translated to another region.
    /// </summary>
    public readonly struct GameAddress
    {
        public GameAddress(string name, long ntscAddress, string dataType = null, long palAddress = 0)
        {
            Name = name;
            NtscAddress = ntscAddress;
            DataType = dataType;
            PalAddress = palAddress;
        }

        public string Name { get; }
        public long NtscAddress { get; }
        public string DataType { get; }

        /// <summary>
        /// The known PAL equivalent of <see cref="NtscAddress"/>.
        /// A value of <c>0</c> means no explicit PAL address is known.
        /// </summary>
        public long PalAddress { get; }

        /// <summary>
        /// Resolves this address for the given region. When an explicit
        /// <see cref="PalAddress"/> is known and PAL is requested, that value is
        /// returned; otherwise the supplied translator is used.
        /// </summary>
        public long Resolve(IAddressTranslator translator, GameRegion region)
        {
            if (region == GameRegion.Pal && PalAddress != 0)
                return PalAddress;

            return translator.Translate(region, NtscAddress);
        }
    }
}
