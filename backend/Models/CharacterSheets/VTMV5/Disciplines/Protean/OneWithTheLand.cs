namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class OneWithTheLand : VampirePower
{
    public override string Id { get; } = "one_with_the_land";
    public override string Name { get; } = "One with the Land";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 5;
    public override string? Prerequisite { get; } = "Earth Meld";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Animalism",
    };
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One day or more, or until physically disturbed";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Sink into the earth of their Domain.";
    public override string? AdditionalNotes { get; } = "Same system as Earth Meld however they are not limited by the material.";
    public override string Source { get; } = "Companion, page 28";
}