namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class ExtinguishVitae : VampireDiscipline
{
    public override string Id { get; } = "extinguish_vitae";
    public override string Name { get; } = "Extinguish Vitae";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Intelligence + Blood Sorcery";
    public override string OpposingPool { get; } = "Stamina + Composure";
    public override string Effect { get; } = "In use, this increases another Kindred’s Hunger.";
    public override string? AdditionalNotes { get; } = "The victim can determine who used this power against them if they can see them and win an Intelligence + Occult vs Wits + Subterfuge roll.";
    public override string Source { get; } = "Corebook, page 272";
}