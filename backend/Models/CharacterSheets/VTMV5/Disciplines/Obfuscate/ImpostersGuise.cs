namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class ImpostersGuise : VampirePower
{
    public override string Id { get; } = "imposters_guise";
    public override string Name { get; } = "Imposter's Guise";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 5;
    public override string? Prerequisite { get; } = "Mask of a Thousand Faces";
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Wits + Obfuscate";
    public override string OpposingPool { get; } = "Manipulation + Performance";
    public override string Effect { get; } = "Appear as someone else.";
    public override string? AdditionalNotes { get; } = "The face they wish to copy must be studied for at least five minutes from multiple angles.";
    public override string Source { get; } = "Corebook, page 263";
}