namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class DreadGaze : VampireDiscipline
{
    public override string Id { get; } = "dread_gaze";
    public override string Name { get; } = "Dread Gaze";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Charisma + Presence";
    public override string OpposingPool { get; } = "Composure + Resolve";
    public override string Effect { get; } = "Instill fear into a target to make them flee.";
    public override string? AdditionalNotes { get; } = "A critical win against a vampire victim means the victim makes a terror Frenzy at difficulty 3.";
    public override string Source { get; } = "Corebook, page 267";
}