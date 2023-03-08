namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class TheftOfVitae : VampirePower
{
    public override string Id { get; } = "theft_of_vitae";
    public override string Name { get; } = "Theft of Vitae";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One feeding";
    public override string DicePool { get; } = "Wits + Blood Sorcery";
    public override string OpposingPool { get; } = "Wits + Occult";
    public override string Effect { get; } = "Manipulate blood from a victim through the air to feed.";
    public override string? AdditionalNotes { get; } = "When in use the victim is under the same influence as a standard Kiss.";
    public override string Source { get; } = "Corebook, page 274";
}