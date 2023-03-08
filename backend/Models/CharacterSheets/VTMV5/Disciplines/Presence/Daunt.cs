namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class Daunt : VampirePower
{
    public override string Id { get; } = "daunt";
    public override string Name { get; } = "Daunt";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene or until ended by the user";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add Presence rating to any intimidation rolls.";
    public override string? AdditionalNotes { get; } = "Awe and Daunt cannot be used at the same time.";
    public override string Source { get; } = "Corebook, page 267";
}