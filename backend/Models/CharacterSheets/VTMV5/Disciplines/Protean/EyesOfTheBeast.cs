namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class EyesOfTheBeast : VampirePower
{
    public override string Id { get; } = "eyes_of_the_beast";
    public override string Name { get; } = "Eyes of the Beast";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "As long as desired";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Allows the user to see in total darkness.";
    public override string? AdditionalNotes { get; } = "+2 bonus dice to intimidation against mortals when active.";
    public override string Source { get; } = "Corebook, page 269";
}