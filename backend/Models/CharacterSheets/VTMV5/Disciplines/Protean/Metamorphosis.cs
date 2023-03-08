namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class Metamorphosis : VampirePower
{
    public override string Id { get; } = "metamorphosis";
    public override string Name { get; } = "Metamorphosis";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Shapechange";
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene unless voluntarily ended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Extends shape change to be able to change into a larger animal than the vampire's mass.";
    public override string? AdditionalNotes { get; } = "Same rules as Shapechange.";
    public override string Source { get; } = "Corebook, page 271";
}