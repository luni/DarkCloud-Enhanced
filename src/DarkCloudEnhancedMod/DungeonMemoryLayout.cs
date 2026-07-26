using DarkCloud.Core.Dungeon;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Provides the memory addresses used by dungeon domain services for the
    /// Dark Cloud Enhanced mod.
    /// </summary>
    internal sealed class DungeonMemoryLayout : IDungeonMemoryLayout, IUngagaDoorMemoryLayout, IUngagaSwapMemoryLayout, IEscapePowderMemoryLayout
    {
        public long BoneDoorOpenTypeAddress => Addresses.BoneDoorOpenType;

        public long GetDoorCheckAddress(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20928670;
                case 4: return 0x2092FA08;
                case 5: return 0x209244AC;
                default: return 0;
            }
        }

        public long GetDoorByte1Address(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20985E0;
                case 4: return 0x2092F978;
                case 5: return 0x2092441C;
                default: return 0;
            }
        }

        public long GetDoorFloat1Address(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20928670;
                case 4: return 0x2092FA08;
                case 5: return 0x209244AC;
                default: return 0;
            }
        }

        public long GetDoorFloat2Address(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20928928;
                case 4: return 0x2092FCC0;
                case 5: return 0x20924764;
                default: return 0;
            }
        }

        public long GetDoorByte2Address(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20928B14;
                case 4: return 0x2092FEAC;
                case 5: return 0x20924920;
                default: return 0;
            }
        }

        public long GetDoorByte3Address(byte dungeon)
        {
            switch (dungeon)
            {
                case 3: return 0x20928AE4;
                case 4: return 0x2092FE7C;
                case 5: return 0x20924950;
                default: return 0;
            }
        }

        public long DungeonIndicatorAddress => 0x202A2010;

        public long ModelLoadedValueDungeon3Address => 0x2193A013;

        public long ModelLoadedValueOtherAddress => 0x217E5453;

        public long ModelWriteAddress1Dungeon3 => 0x2193A013;

        public long ModelWriteAddress2Dungeon3 => 0x2193A014;

        public long ModelWriteAddress1Other => 0x217E5453;

        public long ModelWriteAddress2Other => 0x217E5454;

        public ushort ModelLoadedMagicValue => 12850;

        public System.Collections.Generic.IReadOnlyList<long> ActiveItemAddresses { get; } = new long[] { 0x21CDD8AE, 0x21CDD8B0, 0x21CDD8B2 };

        public System.Collections.Generic.IReadOnlyList<long> ActiveItemCountAddresses { get; } = new long[] { 0x21CDD8B4, 0x21CDD8B6, 0x21CDD8B8 };
    }
}
