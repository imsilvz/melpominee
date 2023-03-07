namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class Chimerstry : VampireDiscipline
{
    public override string Id { get; } = "chimerstry";
    public override string Name { get; } = "Chimerstry";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 1,
        School = "Presence",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Manipulation + Obfuscate";
    public override string OpposingPool { get; } = "Composure + Wits";
    public override string Effect { get; } = "Create brief but realistic hallucinations.";
    public override string? AdditionalNotes { get; } = "These hallucinations can never be recorded or sent through transmissions.";
    public override string Source { get; } = "Companion, page 25";
}