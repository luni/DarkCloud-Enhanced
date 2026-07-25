using System;

namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Status effects that can be applied to a character. The numeric values
    /// match the bit flags used by the game.
    /// </summary>
    [Flags]
    public enum PlayerStatus : ushort
    {
        None = 0,
        NearDeath = 2,
        Freeze = 4,
        Stamina = 8,
        Poison = 16,
        Curse = 32,
        Goo = 64
    }
}
