namespace DarkCloud.Memory.Abstractions
{
    /// <summary>
    /// Small pilot set of symbolic game addresses. This file is intended to be
    /// replaced by a generated source of truth once the address-data generator
    /// is introduced.
    /// </summary>
    public static class KnownGameAddresses
    {
        public static readonly GameAddress BootMarker = new GameAddress("Game.BootMarker", 0x20299540L);
        public static readonly GameAddress PalBootMarker = new GameAddress("Game.PalBootMarker", 0x2029BCA0L);
        public static readonly GameAddress NtscRegionFlag = new GameAddress("Game.NtscRegionFlag", 0x21F10020L);
        public static readonly GameAddress PalRegionFlag = new GameAddress("Game.PalRegionFlag", 0x21F22EA0L);
    }
}
