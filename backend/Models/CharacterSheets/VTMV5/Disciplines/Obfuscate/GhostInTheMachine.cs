namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class GhostInTheMachine : VampirePower
{
    public override string Id { get; } = "ghost_in_the_machine";
    public override string Name { get; } = "Ghost in the Machine";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "As power used";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Allows the effects of Obfuscate to be transmitted through technology when viewed on a live screen.";
    public override string? AdditionalNotes { get; } = "If viewed later the image seems blurred, making identification harder.";
    public override string Source { get; } = "Corebook, page 262";
}