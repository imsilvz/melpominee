namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class PortableShade : VampireDiscipline
{
    public override string Id { get; } = "portable_shade";
    public override string Name { get; } = "Portable Shade";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 1;
    public override string? Prerequisite { get; } = "Sabbat";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Sanguine",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Hours active are equal to the Stamina + Alchemy test or the next sunset, whichever is first";
    public override string DicePool { get; } = "Stamina + Alchemy";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Withstand the effects of Sunlight and walk in the sun.";
    public override string? AdditionalNotes { get; } = "This formula was developed by the Path of the Sun Thin-bloods.";
    public override string Source { get; } = "Sabbat, page 53";
}