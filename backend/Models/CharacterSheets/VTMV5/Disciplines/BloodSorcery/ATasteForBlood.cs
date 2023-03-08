namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class ATasteForBlood : VampirePower
{
    public override string Id { get; } = "a_taste_for_blood";
    public override string Name { get; } = "A Taste for Blood";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Resolve + Blood Sorcery";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Discover traits of another through their blood.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 272";
}