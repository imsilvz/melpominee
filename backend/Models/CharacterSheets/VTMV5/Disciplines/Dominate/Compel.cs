namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class Compel : VampireDiscipline
{
    public override string Id { get; } = "compel";
    public override string Name { get; } = "Compel";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "No more than one scene";
    public override string DicePool { get; } = "Charisma + Dominate";
    public override string OpposingPool { get; } = "Intelligence + Resolve";
    public override string Effect { get; } = "Issue a single command.";
    public override string? AdditionalNotes { get; } = "No rolls are needed when the target is an unprepared mortal. However, mortals who have been Dominated in this scene already or this goes against their nature may roll to resist.";
    public override string Source { get; } = "Corebook, page 256";
}