namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class DisciplineChanneling : VampirePower
{
    public override string Id { get; } = "discipline_channeling";
    public override string Name { get; } = "Discipline Channeling";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Ashfinders, Concoct Ashe";
    public override string Cost { get; } = "The same as the power channeled";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Imbue Ashe with Disciplines";
    public override string? AdditionalNotes { get; } = "Must use the Fixatio method. The Thin-bloods develop this formula within the Ashfinders.";
    public override string Source { get; } = "Cults of the Blood Gods, page 46";
}