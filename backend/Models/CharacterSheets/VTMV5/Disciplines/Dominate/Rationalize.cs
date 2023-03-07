namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class Rationalize : VampireDiscipline
{
    public override string Id { get; } = "rationalize";
    public override string Name { get; } = "Rationalize";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Indefinitely";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Convince victims of Dominate it was their idea the entire time.";
    public override string? AdditionalNotes { get; } = "If pressed on their actions the victim can make a test and if successful they question their actions.";
    public override string Source { get; } = "Corebook, page 257";
}