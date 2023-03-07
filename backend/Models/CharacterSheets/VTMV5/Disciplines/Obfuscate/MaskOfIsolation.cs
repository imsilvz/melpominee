namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class MaskOfIsolation : VampireDiscipline
{
    public override string Id { get; } = "mask_of_isolation";
    public override string Name { get; } = "Mask of Isolation";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Mask of a Thousand Faces";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 1,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One night plus one additional night per margin of success";
    public override string DicePool { get; } = "Manipulation + Obfuscate";
    public override string OpposingPool { get; } = "Charisma + Insight";
    public override string Effect { get; } = "Force Mask of a Thousand Faces onto a victim.";
    public override string? AdditionalNotes { get; } = "Should the user be made aware of the power being used on them the effects end.";
    public override string Source { get; } = "Sabbat, page 48";
}