namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class StarMagnetism : VampireDiscipline
{
    public override string Id { get; } = "star_magnetism";
    public override string Name { get; } = "Star Magnetism";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One additional Rouse Check";
    public override string Duration { get; } = "As power used";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Allows the use of presence through live-feed technology, does not work on recorded content.";
    public override string? AdditionalNotes { get; } = "If using Entrancement the user must speak the targets name clearly.";
    public override string Source { get; } = "Corebook, page 269";
}