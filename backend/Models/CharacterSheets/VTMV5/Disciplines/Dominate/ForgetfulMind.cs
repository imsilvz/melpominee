namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class ForgetfulMind : VampireDiscipline
{
    public override string Id { get; } = "forgetful_mind";
    public override string Name { get; } = "Forgetful Mind";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Indefinitely";
    public override string DicePool { get; } = "Manipulation + Dominate";
    public override string OpposingPool { get; } = "Intelligence + Resolve";
    public override string Effect { get; } = "Rewrite someone's memory.";
    public override string? AdditionalNotes { get; } = "Each point of margin on the test allows one additional memory to be altered.";
    public override string Source { get; } = "Corebook, page 257";
}