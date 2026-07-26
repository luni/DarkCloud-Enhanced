using System.Collections.Generic;
using DarkCloud.Core.Dungeon;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Provides the memory addresses used by dungeon domain services for the
    /// Dark Cloud Enhanced mod.
    /// </summary>
    internal sealed class DungeonMemoryLayout : IDungeonMemoryLayout, IUngagaDoorMemoryLayout, IUngagaSwapMemoryLayout, IEscapePowderMemoryLayout, IMiniBossStaminaMemoryLayout, ISwordOfZeusMemoryLayout, ISideQuestStateMemoryLayout, IFloorSelectionMemoryLayout, IWeaponLevelUpMemoryLayout, IActiveItemMemoryLayout, ISpawnDetectionMemoryLayout, IMiniBossMessageMemoryLayout, ISambaChallengeMemoryLayout, IMayorQuestMemoryLayout
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
                case 3: return 0x209285E0;
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

        public long CurrentCharacterCursorAddress => 0x202A2DE8;

        public long ModelLoadedValueDungeon3Address => 0x2193A013;

        public long ModelLoadedValueOtherAddress => 0x217E5453;

        public long ModelWriteAddress1Dungeon3 => 0x2193A013;

        public long ModelWriteAddress2Dungeon3 => 0x2193A014;

        public long ModelWriteAddress1Other => 0x217E5453;

        public long ModelWriteAddress2Other => 0x217E5454;

        public ushort ModelLoadedMagicValue => 12850;

        public System.Collections.Generic.IReadOnlyList<long> ActiveItemAddresses { get; } = new long[] { 0x21CDD8AE, 0x21CDD8B0, 0x21CDD8B2 };

        public System.Collections.Generic.IReadOnlyList<long> ActiveItemCountAddresses { get; } = new long[] { 0x21CDD8B4, 0x21CDD8B6, 0x21CDD8B8 };

        public IReadOnlyList<MonsterQuestDefinition> MonsterQuestDefinitions { get; } = new MonsterQuestDefinition[]
        {
            new MonsterQuestDefinition("Macho", 0x21CE4406, 0x21CE4405, 0x21CE4402, 2, "You completed Macho's quest!\nWell done!"),
            new MonsterQuestDefinition("Gob", 0x21CE440B, 0x21CE440A, 0x21CE4407, 2, "You completed Gob's quest!\nWell done!"),
            new MonsterQuestDefinition("Jake", 0x21CE4410, 0x21CE440F, 0x21CE440C, 2, "You completed Jake's quest!\nWell done!"),
            new MonsterQuestDefinition("Chief Bonka", 0x21CE4415, 0x21CE4414, 0x21CE4411, 2, "You completed Chief Bonka´s quest!\nWell done!", 35),
        };

        public long GetStaminaTimerAddress(int enemyNumber)
        {
            return Enemies.Enemy0.staminaTimer + (0x190L * enemyNumber);
        }

        public long BackFloorFlagAddress => Addresses.dunBackFloorFlag;

        public long GetWeaponIdAddress(int weaponOffset)
        {
            return Player.Toan.WeaponSlot0.id + (0xF8L * weaponOffset);
        }

        public long GetWeaponThunderAddress(int weaponOffset)
        {
            return Player.Toan.WeaponSlot0.thunder + (0xF8L * weaponOffset);
        }

        public long GetWeaponElementHudAddress(int weaponOffset)
        {
            return Player.Toan.WeaponSlot0.elementHUD + (0xF8L * weaponOffset);
        }

        public long StoredThunderAddress => 0x21CE446D;

        public long MaxAttackAddress => 0x2027B298;

        public long SambaChallengeFlagAddress => 0x21CE445E;

        public long MayorQuestFlagAddress => 0x21CE4468;

        public long MayorQuestFloorAddress => 0x21CE4469;

        public long ButtonInputsAddress => Addresses.buttonInputs;

        public long GildaAddress => Addresses.gilda;

        public long DungeonDebugMenuAddress => Addresses.dungeonDebugMenu;

        public long DungeonModeAddress => Addresses.dungeonMode;

        public long MenuModeAddress => 0x202A2010;

        public long PowerPowderMenuAddress => 0x21D9EC08;

        public long GetWeaponLevelAddress(int index)
        {
            return 0x21CDDA5A + ((long)index * WeaponSlotSize);
        }

        public int WeaponSlotSize => 0xF8;

        public long ActiveItemUsableFlagAddress => 0x21D5676D;

        public long ActiveItemUsableIntAddress => 0x21D56770;

        public long CurrentSlotAddress => 0x202A3598;

        public long ActiveItemBaseAddress => 0x21CDD8AC;

        public int ActiveItemSlotSize => 2;

        public int ActiveItemSlotCount => 3;

        public long AnimationIdAddress => 0x21DC4484;

        public long EscapeFlagAddress => 0x202A35EC;

        public long CurrentCharacterAddress => 0x21CD9550;

        public long CurrentWeaponSlotAddress => 0x21CDD88C;

        public long CurrentWeaponMaxWhpAddress => 0x21EA759C;

        public long GetPowderCountAddress(int slot)
        {
            return 0x21CDD8B2 + (2L * slot);
        }

        public long GetCharacterWeaponWhpAddress(int character, int weaponSlot)
        {
            long baseAddress;
            if (character == 0)
                baseAddress = Player.Toan.WeaponSlot0.whp;
            else if (character == 1)
                baseAddress = Player.Xiao.WeaponSlot0.whp;
            else if (character == 2)
                baseAddress = Player.Goro.WeaponSlot0.whp;
            else if (character == 3)
                baseAddress = Player.Ruby.WeaponSlot0.whp;
            else if (character == 4)
                baseAddress = Player.Ungaga.WeaponSlot0.whp;
            else
                baseAddress = Player.Osmond.WeaponSlot0.whp;

            return baseAddress + ((long)WeaponSlotSize * weaponSlot);
        }

        public long Enemy14RenderStatusAddress => Enemies.Enemy14.renderStatus;

        public long Enemy14HpAddress => Enemies.Enemy14.hp;

        public long Enemy0RenderStatusAddress => Enemies.Enemy0.renderStatus;

        public long HideHudAddress => Addresses.hideHud;

        public long CurrentWeaponIdAddress => 0x21EA7590;

        public long InDungeonFlagAddress => 0x202A34CC;

        public long CurrentAllyAddress => 0x202A3570;

        public long QuestTimerAddress => 0x21CE205C;

        public long CompletionAddress => 0x21CE4462;

        long IMayorQuestMemoryLayout.CompletionAddress => 0x21CE4468;

        public long ExpectedAllyAddress => 0x21CE446A;

        public IReadOnlyList<ushort> AllowedWeaponIds => new ushort[] { 257, 258 };

        public long GetEnemyHpAddress(int index)
        {
            return Enemies.Enemy0.hp + ((long)index * EnemyHpSlotSize);
        }

        public int EnemyHpSlotSize => 0x190;

        public int EnemyCount => 8;
    }
}
