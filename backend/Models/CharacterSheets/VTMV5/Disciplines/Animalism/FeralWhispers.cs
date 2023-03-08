namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class FeralWhispers : VampirePower
{
    public override string Id { get; } = "feral_whispers";
    public override string Name { get; } = "Feral Whispers";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check per type of animal";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Manipulation/Charisma + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Two-way communication with animals or summoning.";
    public override string? AdditionalNotes { get; } = "The cost is free if used on their Famulus.";
    public override string Source { get; } = "Corebook, page 246";
}