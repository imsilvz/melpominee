using Melpominee.app.Models.Characters;
namespace Melpominee.app.Services.Characters;

public class CharacterService
{
    public bool SaveCharacter(int charId, BaseCharacter character)
    {
        return true;
    }

    public T? GetCharacterProperty<T>(int charId) where T : ICharacterSaveable
    {
        // get static load method
        var loadMethod = typeof(T).GetMethod("Load", new Type[] { typeof(int) });
        if (loadMethod is null)
            return default;

        T? character = (T?)loadMethod.Invoke(null, new object[] { charId });
        return character;
    }
}