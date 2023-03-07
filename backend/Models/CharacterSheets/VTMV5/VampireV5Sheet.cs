namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Sheet : BaseCharacterSheet
{
    // header attributes
    public string Name { get; set; } = "";
    public string Concept { get; set; } = "";
    public string Chronicle { get; set; } = ""; // inherit from GameId
    public string Ambition { get; set; } = "";
    public string Desire { get; set; } = "";
    public string Sire { get; set; } = "";
    public int Generation { get; set; } = 13;
    public VampireClan? Clan { get; set; }
    public VampirePredatorType? PredatorType { get; set; }

    public VampireV5Sheet() : base()
    {
        GameType = "VTMV5";
    }

    public VampireV5Sheet(int id) : base(id)
    {
        GameType = "VTMV5";
    }

    public override bool Load()
    {
        throw new NotImplementedException();
    }
}