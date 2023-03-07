namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class TerminalDecree : VampireDiscipline
{
    public override string Id { get; } = "terminal_decree";
    public override string Name { get; } = "Terminal Decree";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Free but will give Stains";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Bolster effects of Dominate to be able to circumvent victims' self-preservation.";
    public override string? AdditionalNotes { get; } = "Terminal commands are always resisted instead of auto failing.";
    public override string Source { get; } = "Corebook, page 257";
}