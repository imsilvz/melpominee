namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class TabulaRasa : VampirePower
{
    public override string Id { get; } = "tabula_rasa";
    public override string Name { get; } = "Tabula Rasa";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Sabbat";
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "Permanent";
    public override string DicePool { get; } = "Resolve + Dominate";
    public override string OpposingPool { get; } = "Composure + Resolve";
    public override string Effect { get; } = "Erase the victim's memory to the point they don't know who they are.";
    public override string? AdditionalNotes { get; } = "Following this power's use is generally a string of lies and Path Indoctrination.";
    public override string Source { get; } = "Sabbat, page 47";
}