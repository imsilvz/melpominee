namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class Toughness : VampireDiscipline
{
    public override string Id { get; } = "toughness";
    public override string Name { get; } = "Toughness";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Subtract the Fortitude rating from all superficial damage taken before halving, it cannot be rounded down below one.";
    public override string? AdditionalNotes { get; } = "This occurs before halving the damage but cannot reduce it below one.";
    public override string Source { get; } = "Corebook, page 258";
}