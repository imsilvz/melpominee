namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class Atavism : VampireDiscipline
{
    public override string Id { get; } = "atavism";
    public override string Name { get; } = "Atavism";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Number of rounds equal to margin +1 or entire scene if it's a Critical win";
    public override string DicePool { get; } = "Composure + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Revert animals to their base and primal instincts.";
    public override string? AdditionalNotes { get; } = "The target must be able to sense the user for this power to work.";
    public override string Source { get; } = "Winter's Teeth #3";
}