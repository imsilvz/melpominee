namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class FleshOfMarble : VampirePower
{
    public override string Id { get; } = "flesh_of_marble";
    public override string Name { get; } = "Flesh of Marble";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Ignore the first source of physical damage each turn unless sunlight.";
    public override string? AdditionalNotes { get; } = "A critical win on an attack bypasses this power.";
    public override string Source { get; } = "Corebook, page 259";
}