namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class Traversal : VampirePower
{
    public override string Id { get; } = "traversal";
    public override string Name { get; } = "Traversal";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Turn";
    public override string DicePool { get; } = "Dexterity + Athletics";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Move fast enough to move up vertical surfaces or liquid surfaces.";
    public override string? AdditionalNotes { get; } = "The Storyteller should inform the user beforehand if the target is too far away";
    public override string Source { get; } = "Corebook, page 253";
}