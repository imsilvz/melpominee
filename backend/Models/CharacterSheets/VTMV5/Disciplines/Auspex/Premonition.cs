namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class Premonition : VampirePower
{
    public override string Id { get; } = "premonition";
    public override string Name { get; } = "Premonition";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "Free / One Rouse Check";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "Resolve + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Visions of the future.";
    public override string? AdditionalNotes { get; } = "The Storyteller may activate this power passively with no cost, when used actively the user must make a Rouse Check and roll Resolve + Auspex.";
    public override string Source { get; } = "Corebook, page 249";
}