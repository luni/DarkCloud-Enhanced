namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Parses the status strings used by the legacy mod into <see cref="PlayerStatus"/> values.
    /// </summary>
    public static class PlayerStatusExtensions
    {
        /// <summary>
        /// Parses a status string such as "freeze" or "poison" into the
        /// corresponding <see cref="PlayerStatus"/>. Returns <see cref="PlayerStatus.None"/>
        /// for unrecognized values.
        /// </summary>
        public static PlayerStatus FromString(string type)
        {
            if (string.IsNullOrEmpty(type))
                return PlayerStatus.None;

            switch (type.ToLowerInvariant())
            {
                case "freeze": return PlayerStatus.Freeze;
                case "stamina": return PlayerStatus.Stamina;
                case "poison": return PlayerStatus.Poison;
                case "curse": return PlayerStatus.Curse;
                case "goo": return PlayerStatus.Goo;
                default: return PlayerStatus.None;
            }
        }
    }
}
