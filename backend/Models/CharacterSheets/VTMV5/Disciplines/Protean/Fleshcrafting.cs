namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class Fleshcrafting : VampirePower
{
    public override string Id { get; } = "fleshcrafting";
    public override string Name { get; } = "Fleshcrafting";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Vicissitude";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Permanent";
    public override string DicePool { get; } = "Resolve + Protean";
    public override string OpposingPool { get; } = "Stamina + Resolve";
    public override string Effect { get; } = "Extends the mastery over the flesh to be used on others.";
    public override string? AdditionalNotes { get; } = "An unwilling subject may resist with the margin of the user's role counting as the number of changes able to be made.";
    public override string Source { get; } = "Companion, page 27";
}