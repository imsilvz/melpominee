namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class Prowess : VampireDiscipline
{
    public override string Id { get; } = "prowess";
    public override string Name { get; } = "Prowess";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add Potence rating to their unarmed damage and to feats of Strength, add half their Potence rating (Rounded up) to Melee damage.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 264";
}