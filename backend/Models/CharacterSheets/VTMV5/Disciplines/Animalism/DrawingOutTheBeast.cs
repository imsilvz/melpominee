namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class DrawingOutTheBeast : VampirePower
{
    public override string Id { get; } = "drawing_out_the_beast";
    public override string Name { get; } = "Drawing Out the Beast";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Frenzy Duration";
    public override string DicePool { get; } = "Wits + Animalism";
    public override string OpposingPool { get; } = "Composure + Resolve";
    public override string Effect { get; } = "Transfer their terror or fury frenzy to a nearby victim.";
    public override string? AdditionalNotes { get; } = "This power cannot transfer Hunger Frenzy.";
    public override string Source { get; } = "Corebook, page 247";
}