namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class ScorpionsTouch : VampirePower
{
    public override string Id { get; } = "scorpions_touch";
    public override string Name { get; } = "Scorpion’s Touch";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One or more Rouse Check";
    public override string Duration { get; } = "One Scene";
    public override string DicePool { get; } = "Strength + Blood Sorcery";
    public override string OpposingPool { get; } = "Stamina + Occult/Fortitude";
    public override string Effect { get; } = "Change own vitae into paralyzing poison.";
    public override string? AdditionalNotes { get; } = "A mortal who takes any damage from this will go unconscious.";
    public override string Source { get; } = "Corebook, page 273";
}