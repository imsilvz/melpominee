namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class HeightenedSenses : VampireDiscipline
{
    public override string Id { get; } = "heightened_senses";
    public override string Name { get; } = "Heightened Senses";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Until Deactivated";
    public override string DicePool { get; } = "Wits + Resolve";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Enhance vampiric senses and add Auspex rating to all perception rolls.";
    public override string? AdditionalNotes { get; } = "Having the power activated for long periods might require the use of Willpower.";
    public override string Source { get; } = "Corebook, page 249";
}