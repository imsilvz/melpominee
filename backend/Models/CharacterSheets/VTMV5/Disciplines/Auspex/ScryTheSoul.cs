namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class ScryTheSoul : VampirePower
{
    public override string Id { get; } = "scry_the_soul";
    public override string Name { get; } = "Scry the Soul";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Intelligence + Auspex";
    public override string OpposingPool { get; } = "Composure + Subterfuge";
    public override string Effect { get; } = "Perceives information about the target.";
    public override string? AdditionalNotes { get; } = "This can either be used on a single target or a crowd.";
    public override string Source { get; } = "Corebook, page 250";
}