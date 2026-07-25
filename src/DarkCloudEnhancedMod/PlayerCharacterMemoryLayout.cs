using System;
using DarkCloud.Core.Players;

namespace DarkCloudEnhancedMod
{
    /// <summary>
    /// Maps <see cref="PlayerCharacterField"/> values to the legacy address
    /// constants in <see cref="Player"/>. This keeps the exact PS2 addresses
    /// out of <see cref="DarkCloud.Core"/> while still reusing the existing
    /// constants during migration.
    /// </summary>
    internal sealed class PlayerCharacterMemoryLayout : IPlayerCharacterMemoryLayout
    {
        public long GetAddress(CharacterType character, PlayerCharacterField field, bool forWrite)
        {
            switch (character)
            {
                case CharacterType.Toan: return GetToanAddress(field, forWrite);
                case CharacterType.Xiao: return GetXiaoAddress(field, forWrite);
                case CharacterType.Goro: return GetGoroAddress(field, forWrite);
                case CharacterType.Ruby: return GetRubyAddress(field, forWrite);
                case CharacterType.Ungaga: return GetUngagaAddress(field, forWrite);
                case CharacterType.Osmond: return GetOsmondAddress(field, forWrite);
                default: throw new ArgumentOutOfRangeException(nameof(character), "Unsupported character type.");
            }
        }

        private static long GetToanAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Toan.hp;
                case PlayerCharacterField.MaxHp: return Player.Toan.maxHP;
                case PlayerCharacterField.Defense: return Player.Toan.defense;
                case PlayerCharacterField.Thirst: return Player.Toan.thirst;
                case PlayerCharacterField.MaxThirst: return forWrite ? Player.Toan.thirstMax : Player.Toan.thirst;
                case PlayerCharacterField.Status: return Player.Toan.status;
                case PlayerCharacterField.StatusTimer: return Player.Toan.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Toan.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        private static long GetXiaoAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Xiao.hp;
                case PlayerCharacterField.MaxHp: return Player.Xiao.maxHP;
                case PlayerCharacterField.Defense: return Player.Xiao.defense;
                case PlayerCharacterField.Thirst: return Player.Xiao.thirst;
                case PlayerCharacterField.MaxThirst: return forWrite ? Player.Xiao.thirstMax : Player.Xiao.thirst;
                case PlayerCharacterField.Status: return Player.Xiao.status;
                case PlayerCharacterField.StatusTimer: return Player.Xiao.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Xiao.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        private static long GetGoroAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Goro.hp;
                case PlayerCharacterField.MaxHp: return Player.Goro.maxHP;
                case PlayerCharacterField.Defense: return Player.Goro.defense;
                case PlayerCharacterField.Thirst: return Player.Goro.thirst;
                case PlayerCharacterField.MaxThirst: return Player.Goro.thirstMax;
                case PlayerCharacterField.Status: return Player.Goro.status;
                case PlayerCharacterField.StatusTimer: return Player.Goro.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Goro.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        private static long GetRubyAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Ruby.hp;
                case PlayerCharacterField.MaxHp: return Player.Ruby.maxHP;
                case PlayerCharacterField.Defense: return Player.Ruby.defense;
                case PlayerCharacterField.Thirst: return Player.Ruby.thirst;
                case PlayerCharacterField.MaxThirst: return forWrite ? Player.Ruby.thirstMax : Player.Ruby.thirst;
                case PlayerCharacterField.Status: return Player.Ruby.status;
                case PlayerCharacterField.StatusTimer: return Player.Ruby.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Ruby.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        private static long GetUngagaAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Ungaga.hp;
                case PlayerCharacterField.MaxHp: return Player.Ungaga.maxHP;
                case PlayerCharacterField.Defense: return Player.Ungaga.defense;
                case PlayerCharacterField.Thirst: return Player.Ungaga.thirst;
                case PlayerCharacterField.MaxThirst: return forWrite ? Player.Ungaga.thirstMax : Player.Ungaga.thirst;
                case PlayerCharacterField.Status: return Player.Ungaga.status;
                case PlayerCharacterField.StatusTimer: return Player.Ungaga.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Ungaga.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }

        private static long GetOsmondAddress(PlayerCharacterField field, bool forWrite)
        {
            switch (field)
            {
                case PlayerCharacterField.Hp: return Player.Osmond.hp;
                case PlayerCharacterField.MaxHp: return Player.Osmond.maxHP;
                case PlayerCharacterField.Defense: return Player.Osmond.defense;
                case PlayerCharacterField.Thirst: return Player.Osmond.thirst;
                case PlayerCharacterField.MaxThirst: return forWrite ? Player.Osmond.thirstMax : Player.Osmond.thirst;
                case PlayerCharacterField.Status: return Player.Osmond.status;
                case PlayerCharacterField.StatusTimer: return Player.Osmond.statusTimer;
                case PlayerCharacterField.WeaponSlot: return Player.Osmond.currentWeaponSlot;
                default: throw new ArgumentOutOfRangeException(nameof(field));
            }
        }
    }
}
