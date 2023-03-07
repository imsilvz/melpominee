namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class MagnumOpus : VampireDiscipline
{
    public override string Id { get; } = "magnum_opus";
    public override string Name { get; } = "Magnum Opus";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 4;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 3,
        School = "Auspex",
    };
    public override string Cost { get; } = "One or more Rouse Checks";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Charisma/Manipulation + Craft";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Infusing Presence into artwork.";
    public override string? AdditionalNotes { get; } = "Audiences must roll Composure + Resolve to resist its effects.";
    public override string Source { get; } = "Winter's Teeth #3";
}