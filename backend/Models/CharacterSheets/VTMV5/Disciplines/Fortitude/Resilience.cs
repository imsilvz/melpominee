namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class Resilience : VampireDiscipline
{
    public override string Id { get; } = "resilience";
    public override string Name { get; } = "Resilience";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add their Fortitude rating to the health track.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 258";
}