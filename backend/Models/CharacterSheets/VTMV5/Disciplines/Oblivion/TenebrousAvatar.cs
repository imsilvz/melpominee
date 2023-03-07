namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class TenebrousAvatar : VampireDiscipline
{
    public override string Id { get; } = "tenebrous_avatar";
    public override string Name { get; } = "Tenebrous Avatar";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One scene or until ended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Changes their body into a shadow able to move over any surface or through small spaces.";
    public override string? AdditionalNotes { get; } = "The user takes no damage except sunlight and fire while in this form.";
    public override string Source { get; } = "Chicago by Night, page 295";
}