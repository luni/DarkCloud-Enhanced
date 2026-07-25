using System;

namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Default implementation of <see cref="IPlayerStateService"/>. It applies
    /// the legacy game's rules for character state while delegating memory
    /// reads and writes to an <see cref="IPlayerStateRepository"/>.
    /// </summary>
    public sealed class PlayerStateService : IPlayerStateService
    {
        private const PlayerStatus AllStatusFlags =
            PlayerStatus.NearDeath |
            PlayerStatus.Freeze |
            PlayerStatus.Stamina |
            PlayerStatus.Poison |
            PlayerStatus.Curse |
            PlayerStatus.Goo;

        private readonly IPlayerStateRepository _repository;

        public PlayerStateService(IPlayerStateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ushort GetHp(CharacterType character)
        {
            if (!_repository.TryReadUInt16(character, PlayerCharacterField.Hp, out ushort value))
                return 0;

            return value;
        }

        public void SetHp(CharacterType character, ushort hp)
        {
            _repository.TryWriteUInt16(character, PlayerCharacterField.Hp, hp);
        }

        public ushort GetMaxHp(CharacterType character)
        {
            if (!_repository.TryReadUInt16(character, PlayerCharacterField.MaxHp, out ushort value))
                return 0;

            return value;
        }

        public void SetMaxHp(CharacterType character, ushort maxHp)
        {
            _repository.TryWriteUInt16(character, PlayerCharacterField.MaxHp, maxHp);
        }

        public void SetMaxHp(CharacterType character, int maxHp)
        {
            // Toan's setter historically wrote four bytes; the others wrote two.
            // Preserve that quirk by character.
            if (character == CharacterType.Toan)
            {
                _repository.TryWriteInt32(character, PlayerCharacterField.MaxHp, maxHp);
            }
            else
            {
                if (maxHp < ushort.MinValue || maxHp > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(maxHp), "Max HP must fit in a 16-bit unsigned value for this character.");

                _repository.TryWriteUInt16(character, PlayerCharacterField.MaxHp, (ushort)maxHp);
            }
        }

        public int GetDefense(CharacterType character)
        {
            if (!_repository.TryReadInt32(character, PlayerCharacterField.Defense, out int value))
                return 0;

            return value;
        }

        public void SetDefense(CharacterType character, int defense)
        {
            _repository.TryWriteInt32(character, PlayerCharacterField.Defense, defense);
        }

        public float GetThirst(CharacterType character)
        {
            if (!_repository.TryReadSingle(character, PlayerCharacterField.Thirst, out float value))
                return 0;

            return value;
        }

        public void SetThirst(CharacterType character, float thirst)
        {
            _repository.TryWriteSingle(character, PlayerCharacterField.Thirst, thirst);
        }

        public float GetMaxThirst(CharacterType character)
        {
            if (!_repository.TryReadSingle(character, PlayerCharacterField.MaxThirst, out float value))
                return 0;

            return value;
        }

        public void SetMaxThirst(CharacterType character, float maxThirst)
        {
            _repository.TryWriteSingle(character, PlayerCharacterField.MaxThirst, maxThirst);
        }

        public PlayerStatus GetStatus(CharacterType character)
        {
            if (!_repository.TryReadUInt16(character, PlayerCharacterField.Status, out ushort raw))
                return PlayerStatus.None;

            PlayerStatus status = (PlayerStatus)raw;

            // Treat any value containing undefined status bits as the absence of status.
            if ((status | AllStatusFlags) != AllStatusFlags)
                return PlayerStatus.None;

            return status;
        }

        public void SetStatus(CharacterType character, string type, ushort timer)
        {
            PlayerStatus status = PlayerStatusExtensions.FromString(type);
            if (status == PlayerStatus.None)
                return;

            _repository.TryWriteUInt16(character, PlayerCharacterField.Status, (ushort)status);
            _repository.TryWriteUInt16(character, PlayerCharacterField.StatusTimer, timer);
        }
    }
}
