namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class WhereTheShroudThins : VampirePower
{
    public override string Id { get; } = "where_the_shroud_thins";
    public override string Name { get; } = "Where the Shroud Thins";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Wits + Oblivion";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Determine the density of the Shroud in their area.";
    public override string? AdditionalNotes { get; } = "The book lists a chart stating the possible different densities and causes, as well as their effects.";
    public override string Source { get; } = "Cults of the Blood Gods, page 205";
}