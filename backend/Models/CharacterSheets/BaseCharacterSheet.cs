namespace Melpominee.app.Models.CharacterSheets;

public abstract class BaseCharacterSheet
{
    public int? Id { get; set; } = null;
    protected string GameType = "base";
    protected string GameId { get; set; } = "";
    protected string PlayerId { get; set; } = "";

    public BaseCharacterSheet() 
    { }

    public abstract bool Save();
}