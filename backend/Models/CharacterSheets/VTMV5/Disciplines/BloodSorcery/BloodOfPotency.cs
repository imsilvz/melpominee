namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class BloodOfPotency : VampirePower
{
    public override string Id { get; } = "blood_of_potency";
    public override string Name { get; } = "Blood of Potency";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Scene or Night";
    public override string DicePool { get; } = "Resolve + Blood Sorcery";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Increase Blood Potency temporarily.";
    public override string? AdditionalNotes { get; } = "This power can allow a Kindred to bypass the Blood Potency limit set by their generation.";
    public override string Source { get; } = "Corebook, page 273";
}