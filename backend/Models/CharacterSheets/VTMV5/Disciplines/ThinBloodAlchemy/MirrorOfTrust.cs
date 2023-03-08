namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class MirrorOfTrust : VampirePower
{
    public override string Id { get; } = "mirror_of_trust";
    public override string Name { get; } = "Mirror of Trust";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 2;
    public override string? Prerequisite { get; } = "Second Inquisition";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Sanguine",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Hour";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Gain 3 extra dice to persuade or intimidate someone into being honest.";
    public override string? AdditionalNotes { get; } = "Thin-bloods develop this formula within the Second Inquisition.";
    public override string Source { get; } = "Second Inquisition, page 46";
}