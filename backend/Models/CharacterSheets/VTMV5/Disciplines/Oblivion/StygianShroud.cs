namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class StygianShroud : VampireDiscipline
{
    public override string Id { get; } = "stygian_shroud";
    public override string Name { get; } = "Stygian Shroud";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Darkness spews out of a nearby shadow and covers the area.";
    public override string? AdditionalNotes { get; } = "The shadow can be extended up to twice the user's Oblivion rating in yards/meters.";
    public override string Source { get; } = "Chicago by Night, page 295";
}