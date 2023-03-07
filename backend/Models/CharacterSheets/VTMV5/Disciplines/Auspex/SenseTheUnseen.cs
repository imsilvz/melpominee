namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class SenseTheUnseen : VampireDiscipline
{
    public override string Id { get; } = "sense_the_unseen";
    public override string Name { get; } = "Sense the Unseen";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "Wits/Resolve + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Sense supernatural activity.";
    public override string? AdditionalNotes { get; } = "If the target is using Obfuscate they oppose using Wits + Obfuscate vs the user's Wits + Auspex, normal searches use Resolve.";
    public override string Source { get; } = "Corebook, page 249";
}