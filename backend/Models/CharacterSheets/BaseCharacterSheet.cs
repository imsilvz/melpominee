namespace Melpominee.app.Models.CharacterSheets;

public abstract class BaseCharacterSheet
{
    public int? Id { get; set; } = null;
    public string? Owner { get; set; } = "";

    public BaseCharacterSheet() 
    { }

    public abstract bool Save();
}