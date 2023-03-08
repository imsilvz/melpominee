namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class Shatter : VampirePower
{
    public override string Id { get; } = "shatter";
    public override string Name { get; } = "Shatter";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Toughness";
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene or until hit";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The opponent takes the damage which Toughness subtracts.";
    public override string? AdditionalNotes { get; } = "Weapons will break if their modifier is met in damage received.";
    public override string Source { get; } = "Cults of the Blood Gods, page 104";
}