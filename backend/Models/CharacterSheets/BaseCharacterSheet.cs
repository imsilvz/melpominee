namespace Melpominee.app.Models.CharacterSheets;

public abstract class BaseCharacterSheet
{
    protected int Id = -1;
    protected string GameType = "base";
    public string GameId { get; set; } = "";
    protected string PlayerId { get; set; } = "";

    public BaseCharacterSheet() 
    { }

    public BaseCharacterSheet(int id)
    {
        Id = id;
        Load();
    }

    public abstract bool Load();
}