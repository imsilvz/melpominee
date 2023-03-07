namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class SplitSecond : VampireDiscipline
{
    public override string Id { get; } = "split_second";
    public override string Name { get; } = "Split Second";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One action, as determined by ST";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Alter the events given by Storyteller in a current scene within reason";
    public override string? AdditionalNotes { get; } = "The action should be reasonable and accomplished within a few seconds in real-time.";
    public override string Source { get; } = "Corebook, page 254";
}