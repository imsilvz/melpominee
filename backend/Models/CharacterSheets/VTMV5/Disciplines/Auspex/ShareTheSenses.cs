namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class ShareTheSenses : VampireDiscipline
{
    public override string Id { get; } = "share_the_senses";
    public override string Name { get; } = "Share the Senses";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Resolve + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Tapping into other's senses.";
    public override string? AdditionalNotes { get; } = "Sense the Unseen can allow the user to be noticed by the victim.";
    public override string Source { get; } = "Corebook, page 250";
}