namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Identifies one of the six playable characters in Dark Cloud, or an
    /// unknown value when the memory read cannot be interpreted.
    /// </summary>
    public enum CharacterType : byte
    {
        Toan = 0,
        Xiao = 1,
        Goro = 2,
        Ruby = 3,
        Ungaga = 4,
        Osmond = 5,
        Unknown = 255
    }
}
