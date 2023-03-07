namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class SoaringLeap : VampireDiscipline
{
    public override string Id { get; } = "soaring_leap";
    public override string Name { get; } = "Soaring Leap";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Leap higher and further than usual.";
    public override string? AdditionalNotes { get; } = "They can move as many meters as three times their Potence rating.";
    public override string Source { get; } = "Corebook, page 264";
}