namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class SenseTheBeast : VampireDiscipline
{
    public override string Id { get; } = "sense_the_beast";
    public override string Name { get; } = "Sense the Beast";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "Resolve + Animalism";
    public override string OpposingPool { get; } = "Composure + Subterfuge";
    public override string Effect { get; } = "Sense hostility and supernatural traits.";
    public override string? AdditionalNotes { get; } = "This power can also be used actively.";
    public override string Source { get; } = "Corebook, page 245";
}