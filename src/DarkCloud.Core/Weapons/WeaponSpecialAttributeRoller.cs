using System;

namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Pure rules for rolling the special-effect attributes on end-game weapons.
    /// The caller supplies a roll function (e.g. <c>Random.Next(100)</c>) so the
    /// rules can be tested deterministically without a real RNG.
    /// </summary>
    public sealed class WeaponSpecialAttributeRoller
    {
        private readonly Func<int> _roll;

        public WeaponSpecialAttributeRoller(Func<int> roll)
        {
            _roll = roll ?? throw new ArgumentNullException(nameof(roll));
        }

        public WeaponEffectValues RollHeavensCloud()
        {
            if (_roll() >= 50)
                return new WeaponEffectValues(0, 0);

            return _roll() < 50
                ? new WeaponEffectValues(32, 0)
                : new WeaponEffectValues(0, 16);
        }

        public WeaponEffectValues RollDarkCloud()
        {
            if (_roll() >= 50)
                return new WeaponEffectValues(0, 0);

            return _roll() < 50
                ? new WeaponEffectValues(32, 0)
                : new WeaponEffectValues(64, 0);
        }

        public WeaponEffectValues RollBigBang()
        {
            if (_roll() >= 50)
                return new WeaponEffectValues(0, 0);

            return _roll() < 50
                ? new WeaponEffectValues(0, 16)
                : new WeaponEffectValues(64, 0);
        }

        public WeaponEffectValues RollAtlamilliaSword()
        {
            if (_roll() >= 50)
                return new WeaponEffectValues(0, 0);

            return _roll() < 50
                ? new WeaponEffectValues(0, 8)
                : new WeaponEffectValues(64, 0);
        }

        public WeaponEffectValues RollDusack()
        {
            return _roll() < 50
                ? new WeaponEffectValues(128, 0)
                : new WeaponEffectValues(0, 0);
        }

        public WeaponEffectValues RollGoddessRing()
        {
            return _roll() < 50
                ? new WeaponEffectValues(0, 8)
                : new WeaponEffectValues(0, 0);
        }

        public WeaponEffectValues RollDestructionRing()
        {
            return _roll() < 50
                ? new WeaponEffectValues(0, 16)
                : new WeaponEffectValues(0, 0);
        }

        public WeaponEffectValues RollSatansRing()
        {
            return _roll() < 50
                ? new WeaponEffectValues(0, 4)
                : new WeaponEffectValues(0, 0);
        }

        public WeaponEffectValues RollSkunk()
        {
            return _roll() < 50
                ? new WeaponEffectValues(32, 0)
                : new WeaponEffectValues(0, 0);
        }

        public WeaponEffectValues RollSwallow()
        {
            return _roll() < 50
                ? new WeaponEffectValues(128, 0)
                : new WeaponEffectValues(0, 0);
        }
    }
}
