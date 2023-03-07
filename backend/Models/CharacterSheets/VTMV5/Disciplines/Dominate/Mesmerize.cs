namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class Mesmerize : VampireDiscipline
{
    public override string Id { get; } = "mesmerize";
    public override string Name { get; } = "Mesmerize";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Until command is carried out or the scene ends";
    public override string DicePool { get; } = "Manipulation + Dominate";
    public override string OpposingPool { get; } = "Intelligence + Resolve";
    public override string Effect { get; } = "Issue complex commands.";
    public override string? AdditionalNotes { get; } = "No rolls are needed when the target is an unprepared mortal. However, if this goes against their nature they may roll to resist.";
    public override string Source { get; } = "Corebook, page 256";
}