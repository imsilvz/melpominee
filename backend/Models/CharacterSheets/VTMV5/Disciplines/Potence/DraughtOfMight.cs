namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class DraughtOfMight : VampireDiscipline
{
    public override string Id { get; } = "draught_of_might";
    public override string Name { get; } = "Draught of Might";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One night";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn their vitae into a Potence boost for others.";
    public override string? AdditionalNotes { get; } = "Each drinker must take one Rouse Checks worth.";
    public override string Source { get; } = "Corebook, page 265";
}