using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
namespace Melpominee.app.Models.Characters;

public interface ICharacterSaveable
{
    public bool Save(int? charId);
}

public interface ICharacterUpdate
{
    public Task Apply(BaseCharacter character, IDistributedCache? cache = null);
}

public abstract class BaseCharacter : ICharacterSaveable
{
    // meta
    [JsonIgnore]
    public bool Loaded = false;
    [JsonIgnore]
    public DateTime LoadedAt = DateTime.MinValue;

    public int? Id { get; set; } = null;
    public string? Owner { get; set; } = "";

    public BaseCharacter() 
    { }

    public abstract bool Save();
    public abstract bool Save(int? charId);
}