namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class AnimalSucculence : VampireDiscipline
{
    public override string Id { get; } = "animal_succulence";
    public override string Name { get; } = "Animal Succulence";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Slake additional hunger from animals and counts Blood Potency as 2 levels lower in regards to slaking penalties.";
    public override string? AdditionalNotes { get; } = "This will never let the character slake to 0.";
    public override string Source { get; } = "Corebook, page 246";
}