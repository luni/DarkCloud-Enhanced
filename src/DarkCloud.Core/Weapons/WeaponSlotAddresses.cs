namespace DarkCloud.Core.Weapons
{
    /// <summary>
    /// Absolute memory addresses for a single weapon slot used by the
    /// synth-sphere upgrade service.
    /// </summary>
    public readonly struct WeaponSlotAddresses
    {
        public WeaponSlotAddresses(
            long level,
            long attack,
            long endurance,
            long speed,
            long magic,
            long slot1ItemId,
            long slot1SynthesisedItemId,
            long hasChangedBySynth,
            long weaponFormerStatsValue)
        {
            Level = level;
            Attack = attack;
            Endurance = endurance;
            Speed = speed;
            Magic = magic;
            Slot1ItemId = slot1ItemId;
            Slot1SynthesisedItemId = slot1SynthesisedItemId;
            HasChangedBySynth = hasChangedBySynth;
            WeaponFormerStatsValue = weaponFormerStatsValue;
        }

        public long Level { get; }
        public long Attack { get; }
        public long Endurance { get; }
        public long Speed { get; }
        public long Magic { get; }
        public long Slot1ItemId { get; }
        public long Slot1SynthesisedItemId { get; }
        public long HasChangedBySynth { get; }
        public long WeaponFormerStatsValue { get; }
    }
}
