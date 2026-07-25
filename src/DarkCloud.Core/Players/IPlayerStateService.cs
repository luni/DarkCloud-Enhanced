namespace DarkCloud.Core.Players
{
    /// <summary>
    /// Domain service for reading and modifying player character state. It
    /// encapsulates the small amount of game logic (such as status-name
    /// mapping) that belongs with the player rules rather than memory layout.
    /// </summary>
    public interface IPlayerStateService
    {
        ushort GetHp(CharacterType character);
        void SetHp(CharacterType character, ushort hp);

        ushort GetMaxHp(CharacterType character);
        void SetMaxHp(CharacterType character, ushort maxHp);
        void SetMaxHp(CharacterType character, int maxHp);

        int GetDefense(CharacterType character);
        void SetDefense(CharacterType character, int defense);

        float GetThirst(CharacterType character);
        void SetThirst(CharacterType character, float thirst);

        float GetMaxThirst(CharacterType character);
        void SetMaxThirst(CharacterType character, float maxThirst);

        PlayerStatus GetStatus(CharacterType character);
        void SetStatus(CharacterType character, string type, ushort timer);
    }
}
