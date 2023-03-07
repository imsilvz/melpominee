namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class AnimalDominion : VampireDiscipline
{
    public override string Id { get; } = "animal_dominion";
    public override string Name { get; } = "Animal Dominion";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One scene or when the directive is fulfilled";
    public override string DicePool { get; } = "Charisma + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Command flocks or packs of animals.";
    public override string? AdditionalNotes { get; } = "This power does not summon animals, instead utilizing those already present.";
    public override string Source { get; } = "Corebook, page 247";
}