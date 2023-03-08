namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class MentalMaze : VampirePower
{
    public override string Id { get; } = "mental_maze";
    public override string Name { get; } = "Mental Maze";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Dominate",
    };
    public override string Cost { get; } = "One or Three Rouse Checks";
    public override string Duration { get; } = "One night";
    public override string DicePool { get; } = "Charisma + Obfuscate";
    public override string OpposingPool { get; } = "Wits + Resolve";
    public override string Effect { get; } = "Remove all sense of direction and location from a victim in a location.";
    public override string? AdditionalNotes { get; } = "The resistance roll can only be made by the supernatural but cannot be teamworked.";
    public override string Source { get; } = "Cults of the Blood Gods, page 85";
}