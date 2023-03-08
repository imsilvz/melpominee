namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class ProwessFromPain : VampirePower
{
    public override string Id { get; } = "prowess_from_pain";
    public override string Name { get; } = "Prowess from Pain";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "No longer suffers penalty from Health damage and can increase one Attribute per level of damage on their tracker.";
    public override string? AdditionalNotes { get; } = "The Attributes may not exceed their Blood Surge value + 6.";
    public override string Source { get; } = "Corebook, page 260";
}