namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class SubsumeTheSpirit : VampirePower
{
    public override string Id { get; } = "subsume_the_spirit";
    public override string Name { get; } = "Subsume the Spirit";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene / Indefinitely";
    public override string DicePool { get; } = "Manipulation + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Possess the body of an animal.";
    public override string? AdditionalNotes { get; } = "The cost is free if used on their Famulus.";
    public override string Source { get; } = "Corebook, page 247";
}