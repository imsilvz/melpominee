namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class WitheringSpirit : VampirePower
{
    public override string Id { get; } = "withering_spirit";
    public override string Name { get; } = "Withering Spirit";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks, Stains";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Resolve + Oblivion";
    public override string OpposingPool { get; } = "Resolve + Occult/Fortitude";
    public override string Effect { get; } = "Erode a victim's spirit till they are a husk.";
    public override string? AdditionalNotes { get; } = "If the target is Impaired, they will not return as a wraith.";
    public override string Source { get; } = "Cults of the Blood Gods, page 208";
}