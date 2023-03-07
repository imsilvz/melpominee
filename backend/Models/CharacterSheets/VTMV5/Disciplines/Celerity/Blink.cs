namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class Blink : VampireDiscipline
{
    public override string Id { get; } = "blink";
    public override string Name { get; } = "Blink";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Turn";
    public override string DicePool { get; } = "Dexterity + Athletics";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Closes the distance as if teleporting.";
    public override string? AdditionalNotes { get; } = "The user moves in a straight line and may need to make checks against difficult terrain.";
    public override string Source { get; } = "Corebook, page 253";
}