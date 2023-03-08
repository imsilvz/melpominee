namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class IrresistibleVoice : VampirePower
{
    public override string Id { get; } = "irresistible_voice";
    public override string Name { get; } = "Irresistible Voice";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 4;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Dominate",
    };
    public override string Cost { get; } = "No additional cost";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user's voice alone is enough to use Dominate on a target.";
    public override string? AdditionalNotes { get; } = "Does not work through technology.";
    public override string Source { get; } = "Corebook, page 268";
}