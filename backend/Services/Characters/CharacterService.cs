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

    public async Task<T?> GetCharacterProperty<T>(params object[] args) where T : ICharacterSaveable
    {
        List<Type> typeList = new List<Type>();
        foreach(var arg in args)
        {
            typeList.Add(arg.GetType());
        }
        typeList.Add(typeof(CharacterService));

        // get static load method
        var loadMethod = typeof(T).GetMethod("Load", typeList.ToArray());
        if (loadMethod is null)
            return default;

        T? character = default;
        var keyName = $"melpominee:character:{args[args.Length - 1]}:{typeof(T).Name}";
        var cacheData = await _cache.GetStringAsync(keyName);
        if (!string.IsNullOrEmpty(cacheData))
        {
            character = JsonSerializer.Deserialize<T>(cacheData);
            if (character is not null)
            {
                return character;
            }
        }

        object[] methodArgs = new object[args.Length + 1];
        Array.Copy(args, methodArgs, args.Length);
        methodArgs[methodArgs.Length - 1] = this;
        character = await (Task<T?>)loadMethod.Invoke(null, methodArgs)!;
        if (character is not null)
        {
            await _cache.SetStringAsync(keyName, JsonSerializer.Serialize<T>(character));
        }
        return character;
    }
}