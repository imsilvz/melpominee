namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class Vanish : VampirePower
{
    public override string Id { get; } = "vanish";
    public override string Name { get; } = "Vanish";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Cloak of Shadows";
    public override string Cost { get; } = "As per power augmented";
    public override string Duration { get; } = "As per power augmented";
    public override string DicePool { get; } = "Wits + Obfuscate";
    public override string OpposingPool { get; } = "Wits + Awareness";
    public override string Effect { get; } = "Activate Cloak of Shadows or Unseen Passage while being observed.";
    public override string? AdditionalNotes { get; } = "This power makes the memory of the Kindred foggy and indistinct, but it will not affect the memories of vampires.";
    public override string Source { get; } = "Corebook, page 262";
}