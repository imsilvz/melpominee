namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class BrutalFeed : VampirePower
{
    public override string Id { get; } = "brutal_feed";
    public override string Name { get; } = "Brutal Feed";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One feeding";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn feeding into a violent and messy affair that only lasts seconds to Slake the user's Hunger.";
    public override string? AdditionalNotes { get; } = "Against vampires the number of feeding actions is halved (rounded down).";
    public override string Source { get; } = "Corebook, page 264";
}