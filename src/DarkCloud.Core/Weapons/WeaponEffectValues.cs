namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// The special-effect byte values for a weapon.
    /// </summary>
    public readonly struct WeaponEffectValues
    {
        public WeaponEffectValues(byte effect, byte effect2)
        {
            Effect = effect;
            Effect2 = effect2;
        }

        /// <summary>
        /// Base weapon special effects (Set 1):
        /// 2=Big bucks, 4=poor, 8=quench, 16=thirst, 32=poison, 64=stop, 128=steal.
        /// </summary>
        public byte Effect { get; }

        /// <summary>
        /// Base weapon special effects (Set 2):
        /// 1=fragile, 2=durable, 4=drain, 8=heal, 16=critical, 32=absup.
        /// </summary>
        public byte Effect2 { get; }
    }
}
