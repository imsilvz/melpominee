namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class CatsGrace : VampireDiscipline
{
    public override string Id { get; } = "cats_grace";
    public override string Name { get; } = "Cat’s Grace";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Automatically pass balance tests.";
    public override string? AdditionalNotes { get; } = "Does not work on objects that cannot support their weight.";
    public override string Source { get; } = "Corebook, page 252";
}