namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class FortifyTheInnerFaçade : VampireDiscipline
{
    public override string Id { get; } = "fortify_the_inner_façade";
    public override string Name { get; } = "Fortify the Inner Façade";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Increasing the Difficulty of mental powers to read or pierce the mind by half of the Fortitude Rating.";
    public override string? AdditionalNotes { get; } = "If the rules allow them to resist, they add their Fortitude rating to their pool instead.";
    public override string Source { get; } = "Corebook, page 259";
}