namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class TheUnfetteredHeart : VampirePower
{
    public override string Id { get; } = "the_unfettered_heart";
    public override string Name { get; } = "The Unfettered Heart";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The heart of the vampire can move freely within the chest, making staking more difficult.";
    public override string? AdditionalNotes { get; } = "Only upon a critical win does the stake penetrate when in melee combat.";
    public override string Source { get; } = "Corebook, page 271";
}