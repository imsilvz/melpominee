namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class ShadowCast : VampireDiscipline
{
    public override string Id { get; } = "shadow_cast";
    public override string Name { get; } = "Shadow Cast";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Conjure shadows from the user's body.";
    public override string? AdditionalNotes { get; } = "The shadow can be extended up to twice the user's Oblivion rating in yards/meters. Those standing in the shadow take more Willpower damage from social conflict";
    public override string Source { get; } = "Chicago by Night, page 293";
}