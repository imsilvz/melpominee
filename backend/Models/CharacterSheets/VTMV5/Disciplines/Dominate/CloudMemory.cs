namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class CloudMemory : VampireDiscipline
{
    public override string Id { get; } = "cloud_memory";
    public override string Name { get; } = "Cloud Memory";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Indefinitely";
    public override string DicePool { get; } = "Charisma + Dominate";
    public override string OpposingPool { get; } = "Wits + Resolve";
    public override string Effect { get; } = "Make someone forget the current moment.";
    public override string? AdditionalNotes { get; } = "No rolls are needed when the target is an unprepared mortal.";
    public override string Source { get; } = "Corebook, page 256";
}