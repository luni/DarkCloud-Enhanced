using System;

namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Translates addresses between NTSC and PAL layouts using a sorted NTSC
    /// to PAL mapping table.
    /// </summary>
    public sealed class RegionAddressTranslator : IAddressTranslator
    {
        private readonly long[] _ntsc;
        private readonly long[] _pal;

        public RegionAddressTranslator(long[] ntsc, long[] pal)
        {
            if (ntsc == null)
                throw new ArgumentNullException(nameof(ntsc));
            if (pal == null)
                throw new ArgumentNullException(nameof(pal));
            if (ntsc.Length != pal.Length)
                throw new ArgumentException("NTSC and PAL tables must have the same length.");

            _ntsc = new long[ntsc.Length];
            _pal = new long[pal.Length];
            Array.Copy(ntsc, _ntsc, ntsc.Length);
            Array.Copy(pal, _pal, pal.Length);
            Array.Sort(_ntsc, _pal);
        }

        public long Translate(GameRegion region, long ntscAddress)
        {
            if (region != GameRegion.Pal || _ntsc.Length == 0)
                return ntscAddress;

            int index = Array.BinarySearch(_ntsc, ntscAddress);
            if (index >= 0)
                return _pal[index];

            // If not an exact match, find the nearest preceding mapped address and apply its delta.
            index = ~index - 1;
            if (index >= 0 && index < _ntsc.Length)
                return checked(ntscAddress + (_pal[index] - _ntsc[index]));

            return ntscAddress;
        }
    }
}
