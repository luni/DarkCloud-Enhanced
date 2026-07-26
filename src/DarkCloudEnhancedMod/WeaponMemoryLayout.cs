using DarkCloud.Core.Weapons;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// In-memory layout for the base weapon database.
    /// </summary>
    internal sealed class WeaponMemoryLayout : IWeaponMemoryLayout
    {
        public int WeaponOffset => Weapons.weaponoffset;

        public long LambTransformThresholdAddress => Weapons.lambTransformThreshold;

        public long LambStatsThresholdAddress => Weapons.lambStatsThreshold;

        public int GetCharacterOffset(WeaponCharacter character)
        {
            switch (character)
            {
                case WeaponCharacter.Toan: return 0;
                case WeaponCharacter.Xiao: return Weapons.xiaooffset;
                case WeaponCharacter.Goro: return Weapons.gorooffset;
                case WeaponCharacter.Ruby: return Weapons.rubyoffset;
                case WeaponCharacter.Ungaga: return Weapons.ungagaoffset;
                case WeaponCharacter.Osmond: return Weapons.osmondoffset;
                default: return 0;
            }
        }

        public long GetStatBaseAddress(WeaponStat stat)
        {
            switch (stat)
            {
                case WeaponStat.SynthSlot1: return Weapons.synth1;
                case WeaponStat.SynthSlot2: return Weapons.synth2;
                case WeaponStat.SynthSlot3: return Weapons.synth3;
                case WeaponStat.SynthSlot4: return Weapons.synth4;
                case WeaponStat.SynthSlot5: return Weapons.synth5;
                case WeaponStat.SynthSlot6: return Weapons.synth6;
                case WeaponStat.Ownership: return Weapons.ownership;
                case WeaponStat.WHP: return Weapons.whp;
                case WeaponStat.ABS: return Weapons.abs;
                case WeaponStat.ABSAdd: return Weapons.absadd;
                case WeaponStat.Attack: return Weapons.attack;
                case WeaponStat.MaxAttack: return Weapons.maxattack;
                case WeaponStat.Endurance: return Weapons.endurance;
                case WeaponStat.Speed: return Weapons.speed;
                case WeaponStat.Magic: return Weapons.magic;
                case WeaponStat.MaxMagic: return Weapons.maxmagic;
                case WeaponStat.Fire: return Weapons.fire;
                case WeaponStat.Ice: return Weapons.ice;
                case WeaponStat.Thunder: return Weapons.thunder;
                case WeaponStat.Wind: return Weapons.wind;
                case WeaponStat.Holy: return Weapons.holy;
                case WeaponStat.DinoSlayer: return Weapons.dinoslayer;
                case WeaponStat.UndeadBuster: return Weapons.undead;
                case WeaponStat.SeaKiller: return Weapons.sea;
                case WeaponStat.StoneBreaker: return Weapons.stone;
                case WeaponStat.PlantBuster: return Weapons.plant;
                case WeaponStat.BeastBuster: return Weapons.beast;
                case WeaponStat.SkyHunter: return Weapons.sky;
                case WeaponStat.MetalBreaker: return Weapons.metal;
                case WeaponStat.MimicBreaker: return Weapons.mimic;
                case WeaponStat.MageSlayer: return Weapons.mage;
                case WeaponStat.Effect: return Weapons.effect;
                case WeaponStat.Effect2: return Weapons.effect2;
                case WeaponStat.BuildUp: return Weapons.buildup;
                default: return 0;
            }
        }
    }
}
