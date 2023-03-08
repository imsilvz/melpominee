namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class Awe : VampirePower
{
    public override string Id { get; } = "awe";
    public override string Name { get; } = "Awe";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene or until ended by the user";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add Presence rating to any Skill roll involving Persuasion, Performance, or Charisma related rolls as per ST discretion.";
    public override string? AdditionalNotes { get; } = "Once the power wears off the victim reverts to their original opinions.";
    public override string Source { get; } = "Corebook, page 267";
}