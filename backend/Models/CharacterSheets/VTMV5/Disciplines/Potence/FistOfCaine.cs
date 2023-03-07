namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class FistOfCaine : VampireDiscipline
{
    public override string Id { get; } = "fist_of_caine";
    public override string Name { get; } = "Fist of Caine";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Inflict Aggravated Health damage to mortals and supernatural creatures alike.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 266";
}