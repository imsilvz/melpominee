namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class Dementation : VampirePower
{
    public override string Id { get; } = "dementation";
    public override string Name { get; } = "Dementation";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Obfuscate",
    };
    public override string Cost { get; } = "One Rouse Check per target per Scene";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Manipulation + Dominate";
    public override string OpposingPool { get; } = "Composure + Intelligence";
    public override string Effect { get; } = "Drive others insane.";
    public override string? AdditionalNotes { get; } = "To use this power, the user must have had a conversation with them.";
    public override string Source { get; } = "Corebook, page 256";
}