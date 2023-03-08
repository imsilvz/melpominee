namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class UnswayableMind : VampirePower
{
    public override string Id { get; } = "unswayable_mind";
    public override string Name { get; } = "Unswayable Mind";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add Fortitude rating to rolls to resist methods to sway the character's mind against their will including supernatural.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 258";
}