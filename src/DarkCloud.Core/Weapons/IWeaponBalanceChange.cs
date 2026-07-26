namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// A single balance change that can be applied through a <see cref="WeaponStatService"/>.
    /// </summary>
    public interface IWeaponBalanceChange
    {
        void Apply(WeaponStatService statService);
    }
}
