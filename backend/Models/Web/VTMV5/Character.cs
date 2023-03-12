using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class CharacterSheetResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Character? Character { get; set; }
}

public class VampireCharacterUpdate
{
    public string? Name { get; set; }
    public string? Concept { get; set; }
    public string? Chronicle { get; set; }
    public string? Ambition { get; set; }
    public string? Desire { get; set; }
    public string? Sire { get; set; }
    public int? Generation { get; set; }
    public string? Clan { get; set; }
    public string? PredatorType { get; set; }
    public void Apply(VampireV5Character character)
    {
        character.Name = Name ?? character.Name;
        character.Concept = Concept ?? character.Concept;
        character.Chronicle = Chronicle ?? character.Chronicle;
        character.Ambition = Ambition ?? character.Ambition;
        character.Desire = Desire ?? character.Desire;
        character.Sire = Sire ?? character.Sire;
        character.Generation = Generation ?? character.Generation;
        character.Clan = Clan is not null ? 
            VampireClan.GetClan(Clan) : character.Clan;
        character.PredatorType = PredatorType is not null ? 
            VampirePredatorType.GetPredatorType(PredatorType) : character.PredatorType;
        character.Save();
    }
}