namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class ScentOfPrey : VampirePower
{
    public override string Id { get; } = "scent_of_prey";
    public override string Name { get; } = "Scent of Prey";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Resolve + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Track a mortal down who has witnessed a masquerade breach.";
    public override string? AdditionalNotes { get; } = "It lasts one night if Critical Win.";
    public override string Source { get; } = "Sabbat, page 47";
}