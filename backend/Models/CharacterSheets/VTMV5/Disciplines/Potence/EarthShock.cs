namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class EarthShock : VampireDiscipline
{
    public override string Id { get; } = "earth_shock";
    public override string Name { get; } = "Earth Shock";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One use";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Create a shockwave to throw opponents prone.";
    public override string? AdditionalNotes { get; } = "This can only be used once per scene.";
    public override string Source { get; } = "Corebook, page 265";
}