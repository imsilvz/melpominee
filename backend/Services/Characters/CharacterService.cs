using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Models.Characters;
namespace Melpominee.app.Services.Characters;

public class CharacterService
{
    private readonly IDistributedCache _cache;
    public CharacterService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public bool SaveCharacterProperty<T>(int charId, T property)
    {
        return true;
    }

    public async Task<T?> GetCharacterProperty<T>(int charId) where T : ICharacterSaveable
    {
        // get static load method
        var loadMethod = typeof(T).GetMethod("Load", new Type[] { typeof(int) });
        if (loadMethod is null)
            return default;

        T? character = default;
        var keyName = $"melpominee:character:{charId}:{typeof(T).Name}";
        var cacheData = await _cache.GetStringAsync(keyName);
        if (!string.IsNullOrEmpty(cacheData))
        {
            character = JsonSerializer.Deserialize<T>(cacheData);
            Console.WriteLine(character);
            if (character is not null)
            {
                return character;
            }
        }

        character = (T?)loadMethod.Invoke(null, new object[] { charId });
        if (character is not null)
        {
            await _cache.SetStringAsync(keyName, JsonSerializer.Serialize<T>(character));
            Console.WriteLine(await _cache.GetStringAsync(keyName));
        }
        return character;
    }
}