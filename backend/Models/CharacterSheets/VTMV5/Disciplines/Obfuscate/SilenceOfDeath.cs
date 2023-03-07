namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class SilenceOfDeath : VampireDiscipline
{
    public override string Id { get; } = "silence_of_death";
    public override string Name { get; } = "Silence of Death";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Nullifies the sounds a user makes.";
    public override string? AdditionalNotes { get; } = "This power does not eliminate powers made outside of the user's personal space.";
    public override string Source { get; } = "*  Corebook, page 261";
}