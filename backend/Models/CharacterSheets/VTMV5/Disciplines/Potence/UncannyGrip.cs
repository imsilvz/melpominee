namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class UncannyGrip : VampireDiscipline
{
    public override string Id { get; } = "uncanny_grip";
    public override string Name { get; } = "Uncanny Grip";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Strengthens their grip on most surfaces, allowing them to climb or hang unsupported.";
    public override string? AdditionalNotes { get; } = "The use of this power leaves obvious traces from the damage caused.";
    public override string Source { get; } = "Corebook, page 265";
}