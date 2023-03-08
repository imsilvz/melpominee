namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class OblivionSight : VampirePower
{
    public override string Id { get; } = "oblivion_sight";
    public override string Name { get; } = "Oblivion Sight";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "See in darkness clearly and see ghosts present.";
    public override string? AdditionalNotes { get; } = "This does not give the ability to make physical contact with ghosts.";
    public override string Source { get; } = "Chicago by Night, page 293";
}