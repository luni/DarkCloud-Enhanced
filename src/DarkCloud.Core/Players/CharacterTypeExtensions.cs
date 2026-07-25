namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Provides the display name for each <see cref="CharacterType" /> value.
    /// </summary>
    public static class CharacterTypeExtensions
    {
        /// <summary>
        /// Returns the English display name for a character, or <c>null</c> if
        /// the character value is not recognized.
        /// </summary>
        public static string GetName(this CharacterType character)
        {
            switch (character)
            {
                case CharacterType.Toan: return "Toan";
                case CharacterType.Xiao: return "Xiao";
                case CharacterType.Goro: return "Goro";
                case CharacterType.Ruby: return "Ruby";
                case CharacterType.Ungaga: return "Ungaga";
                case CharacterType.Osmond: return "Osmond";
                default: return null;
            }
        }
    }
}
