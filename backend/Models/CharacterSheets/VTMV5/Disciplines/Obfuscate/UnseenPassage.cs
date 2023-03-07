namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class UnseenPassage : VampireDiscipline
{
    public override string Id { get; } = "unseen_passage";
    public override string Name { get; } = "Unseen Passage";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene or until detection";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user can now move while remaining hidden.";
    public override string? AdditionalNotes { get; } = "This power will fail if the user is being actively watched when activated.";
    public override string Source { get; } = "Corebook, page 261";
}