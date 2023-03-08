namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class Shapechange : VampirePower
{
    public override string Id { get; } = "shapechange";
    public override string Name { get; } = "Shapechange";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene unless voluntarily ended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Change into an animal with a similar body mass.";
    public override string? AdditionalNotes { get; } = "Users gain the Physical Attributes and other traits of the animal they've changed into.";
    public override string Source { get; } = "Corebook, page 270";
}