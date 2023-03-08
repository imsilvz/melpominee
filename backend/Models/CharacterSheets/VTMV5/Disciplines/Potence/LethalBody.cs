namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class LethalBody : VampirePower
{
    public override string Id { get; } = "lethal_body";
    public override string Name { get; } = "Lethal Body";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Unarmed attacks do Aggravated Health damage to mortals when used and ignore one level of armor per Potence rating of user.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Corebook, page 264";
}