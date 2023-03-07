namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class WeightOfTheFeather : VampireDiscipline
{
    public override string Id { get; } = "weight_of_the_feather";
    public override string Name { get; } = "Weight of the Feather";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "As long as desired";
    public override string DicePool { get; } = "Wits + Survival";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user can make themselves almost weightless.";
    public override string? AdditionalNotes { get; } = "Wits + Survival is only used when activated as a reaction.";
    public override string Source { get; } = "Corebook, page 269";
}