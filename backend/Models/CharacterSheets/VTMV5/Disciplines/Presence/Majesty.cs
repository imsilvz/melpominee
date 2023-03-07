namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class Majesty : VampireDiscipline
{
    public override string Id { get; } = "majesty";
    public override string Name { get; } = "Majesty";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Charisma + Presence";
    public override string OpposingPool { get; } = "Composure + Resolve";
    public override string Effect { get; } = "Everyone who looks at the user is dumbstruck unable to act in any way other than self-preservation.";
    public override string? AdditionalNotes { get; } = "A win on the contested roll allows one turn plus one per margin of freedom.";
    public override string Source { get; } = "Corebook, page 268";
}