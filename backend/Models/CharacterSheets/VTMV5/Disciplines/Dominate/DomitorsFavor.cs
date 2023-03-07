namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class DomitorsFavor : VampireDiscipline
{
    public override string Id { get; } = "domitors_favor";
    public override string Name { get; } = "Domitor's Favor";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One month";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Make defiance while under a Blood Bond more difficult.";
    public override string? AdditionalNotes { get; } = "Total fail on defiance rolls means the bond does not weaken that month.";
    public override string Source { get; } = "Companion, page 25";
}