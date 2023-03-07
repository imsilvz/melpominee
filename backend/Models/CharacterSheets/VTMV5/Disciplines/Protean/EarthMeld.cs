namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class EarthMeld : VampireDiscipline
{
    public override string Id { get; } = "earth_meld";
    public override string Name { get; } = "Earth Meld";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One day or more, or until disturbed";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Sink into the earth and become one with the soil.";
    public override string? AdditionalNotes { get; } = "This power only works on natural surfaces and not artificial such as concrete.";
    public override string Source { get; } = "Corebook, page 270";
}