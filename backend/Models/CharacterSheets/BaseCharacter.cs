using System.Text.Json.Serialization;
namespace Melpominee.app.Models.CharacterSheets;

public abstract class BaseCharacter
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
}