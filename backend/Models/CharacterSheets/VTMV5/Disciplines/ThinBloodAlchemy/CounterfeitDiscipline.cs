namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class CounterfeitDiscipline : VampirePower
{
    public override string Id { get; } = "counterfeit_discipline";
    public override string Name { get; } = "Counterfeit Discipline";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "The same as the power channeled";
    public override string Duration { get; } = "The same as the power channeled";
    public override string DicePool { get; } = "The same as the power channeled";
    public override string OpposingPool { get; } = "The same as the power channeled";
    public override string Effect { get; } = "Counterfeit a four-dot Discipline";
    public override string? AdditionalNotes { get; } = "Requires a drop of vitae from a vampire of a matching clan or who possesses the Discipline.";
    public override string Source { get; } = "Corebook, page 287";
}