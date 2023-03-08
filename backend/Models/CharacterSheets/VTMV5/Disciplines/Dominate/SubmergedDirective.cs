namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class SubmergedDirective : VampirePower
{
    public override string Id { get; } = "submerged_directive";
    public override string Name { get; } = "Submerged Directive";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Mesmerize";
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Implant Dominate orders as suggestions for victims.";
    public override string? AdditionalNotes { get; } = "These orders never expire until completed and targets can only have one at a time.";
    public override string Source { get; } = "Corebook, page 257";
}