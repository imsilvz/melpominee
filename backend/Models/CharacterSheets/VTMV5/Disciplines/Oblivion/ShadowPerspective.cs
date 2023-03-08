namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class ShadowPerspective : VampirePower
{
    public override string Id { get; } = "shadow_perspective";
    public override string Name { get; } = "Shadow Perspective";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Up to one scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Projects their senses into a shadow within line of sight.";
    public override string? AdditionalNotes { get; } = "The use of this power is undetectable in the shadow other than by supernatural means such as Sense the Unseen.";
    public override string Source { get; } = "Chicago by Night, page 294";
}