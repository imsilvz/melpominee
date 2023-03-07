namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class TouchOfOblivion : VampireDiscipline
{
    public override string Id { get; } = "touch_of_oblivion";
    public override string Name { get; } = "Touch of Oblivion";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Wither's a body part on touch.";
    public override string? AdditionalNotes { get; } = "If the victim is attempting to avoid the vampire it may require a contested Strength + Brawl roll.";
    public override string Source { get; } = "Chicago by Night, page 294";
}