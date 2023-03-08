namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class Entrancement : VampirePower
{
    public override string Id { get; } = "entrancement";
    public override string Name { get; } = "Entrancement";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One hour plus one per point of margin";
    public override string DicePool { get; } = "Charisma + Presence";
    public override string OpposingPool { get; } = "Composure + Wits";
    public override string Effect { get; } = "Influence someone into a star-struck or beguiled state of mind where they do their best to keep the user happy. Adding their Presence rating in dice to any social rolls against the victim.";
    public override string? AdditionalNotes { get; } = "If the request is harmful to the victim or their loved ones, or opposes their tenets the test must be made again or Entrancement fails.";
    public override string Source { get; } = "Corebook, page 268";
}