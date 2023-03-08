namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class SpiritsTouch : VampirePower
{
    public override string Id { get; } = "spirits_touch";
    public override string Name { get; } = "Spirit's Touch";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Intelligence + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Gathering emotional residue from an object or location";
    public override string? AdditionalNotes { get; } = "What information they gleam works from the most recent backward.";
    public override string Source { get; } = "Corebook, page 250";
}