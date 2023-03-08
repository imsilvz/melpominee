namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class SkuldFulfilled : VampirePower
{
    public override string Id { get; } = "skuld_fulfilled";
    public override string Name { get; } = "Skuld Fulfilled";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "Variable, depending on if the condition is treatable";
    public override string DicePool { get; } = "Stamina + Oblivion";
    public override string OpposingPool { get; } = "Stamina + Medicine/Fortitude";
    public override string Effect { get; } = "Reintroduce illnesses someone has recovered from.";
    public override string? AdditionalNotes { get; } = "If the victim is a ghoul their immunity to aging is removed and eliminates any vitae in their system.";
    public override string Source { get; } = "Cults of the Blood Gods, page 207";
}