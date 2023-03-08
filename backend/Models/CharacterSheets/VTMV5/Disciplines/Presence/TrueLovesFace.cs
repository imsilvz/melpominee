namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class TrueLovesFace : VampirePower
{
    public override string Id { get; } = "true_loves_face";
    public override string Name { get; } = "True Love's Face";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Church of Set";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 3,
        School = "Obfuscate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Manipulation + Presence";
    public override string OpposingPool { get; } = "Composure + Wits";
    public override string Effect { get; } = "The victim will perceive the user as a mortal they have strong emotional ties with, be it hatred or love.";
    public override string? AdditionalNotes { get; } = "Can net stains for the victim if the perceived target is their touchstone.";
    public override string Source { get; } = "Cults of the Blood Gods, page 85";
}