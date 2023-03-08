namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class ShadowStep : VampirePower
{
    public override string Id { get; } = "shadow_step";
    public override string Name { get; } = "Shadow Step";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user can step into one shadow and appear in another within their sight.";
    public override string? AdditionalNotes { get; } = "A willing person may be taken through the Shadow Step but should the user stain, so does the passenger.";
    public override string Source { get; } = "Chicago by Night, page 295";
}