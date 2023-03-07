namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class Vicissitude : VampireDiscipline
{
    public override string Id { get; } = "vicissitude";
    public override string Name { get; } = "Vicissitude";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Permanent";
    public override string DicePool { get; } = "Resolve + Protean";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Sculpt the flesh of bodies allowing changes to their own bodies.";
    public override string? AdditionalNotes { get; } = "Each success on the roll allows a single change to be made.";
    public override string Source { get; } = "Companion, page 27";
}