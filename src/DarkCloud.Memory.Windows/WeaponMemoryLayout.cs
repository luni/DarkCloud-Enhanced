using DarkCloud.Core.Weapons;

namespace DarkCloud.Memory.Windows
{
    /// <summary>
    /// In-memory layout for the base weapon database. The constants mirror the
    /// legacy <c>DarkCloudEnhancedMod.Weapons</c> offsets so the shared memory
    /// layer can use them without taking a dependency on the legacy host.
    /// </summary>
    internal sealed class WeaponMemoryLayout : IWeaponMemoryLayout
    {
        public int WeaponOffset => 0x4C;

        public long LambTransformThresholdAddress => 0x202A1818L;

        public long LambStatsThresholdAddress => 0x202A188CL;

        public int GetCharacterOffset(WeaponCharacter character)
        {
            switch (character)
            {
                case WeaponCharacter.Toan: return 0;
                case WeaponCharacter.Xiao: return 0xC78;
                case WeaponCharacter.Goro: return 0x10EC;
                case WeaponCharacter.Ruby: return 0x15F8;
                case WeaponCharacter.Ungaga: return 0x1AB8;
                case WeaponCharacter.Osmond: return 0x1F78;
                default: return 0;
            }
        }

        public long GetStatBaseAddress(WeaponStat stat)
        {
            switch (stat)
            {
                case WeaponStat.SynthSlot1: return 0x2027A717;
                case WeaponStat.SynthSlot2: return 0x2027A718;
                case WeaponStat.SynthSlot3: return 0x2027A719;
                case WeaponStat.SynthSlot4: return 0x2027A71A;
                case WeaponStat.SynthSlot5: return 0x2027A71B;
                case WeaponStat.SynthSlot6: return 0x2027A71C;
                case WeaponStat.Ownership: return 0x2027A716;
                case WeaponStat.WHP: return 0x2027A70C;
                case WeaponStat.ABS: return 0x2027A73C;
                case WeaponStat.ABSAdd: return 0x2027A73E;
                case WeaponStat.Attack: return 0x2027A70E;
                case WeaponStat.MaxAttack: return 0x2027A750;
                case WeaponStat.Endurance: return 0x2027A710;
                case WeaponStat.Speed: return 0x2027A712;
                case WeaponStat.Magic: return 0x2027A714;
                case WeaponStat.MaxMagic: return 0x2027A752;
                case WeaponStat.Fire: return 0x2027A71E;
                case WeaponStat.Ice: return 0x2027A720;
                case WeaponStat.Thunder: return 0x2027A722;
                case WeaponStat.Wind: return 0x2027A724;
                case WeaponStat.Holy: return 0x2027A726;
                case WeaponStat.DinoSlayer: return 0x2027A728;
                case WeaponStat.UndeadBuster: return 0x2027A72A;
                case WeaponStat.SeaKiller: return 0x2027A72C;
                case WeaponStat.StoneBreaker: return 0x2027A72E;
                case WeaponStat.PlantBuster: return 0x2027A730;
                case WeaponStat.BeastBuster: return 0x2027A732;
                case WeaponStat.SkyHunter: return 0x2027A734;
                case WeaponStat.MetalBreaker: return 0x2027A736;
                case WeaponStat.MimicBreaker: return 0x2027A738;
                case WeaponStat.MageSlayer: return 0x2027A73A;
                case WeaponStat.Effect: return 0x2027A744;
                case WeaponStat.Effect2: return 0x2027A745;
                case WeaponStat.BuildUp: return 0x2027A748;
                default: return 0;
            }
        }
    }
}
