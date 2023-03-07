namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class ConcoctAshe : VampireDiscipline
{
    public override string Id { get; } = "concoct_ashe";
    public override string Name { get; } = "Concoct Ashe";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Ashfinders";
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Intelligence + Alchemy";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Create Ashe from the remains of a vampire.";
    public override string? AdditionalNotes { get; } = "Must use the Fixatio method. The Thin-bloods develop this formula within the Ashfinders.";
    public override string Source { get; } = "Cults of the Blood Gods, page 45";
}