namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class AirborneMomentum : VampirePower
{
    public override string Id { get; } = "airborne_momentum";
    public override string Name { get; } = "Airborne Momentum";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 4;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Choleric, Sanguine",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Strength + Alchemy";
    public override string OpposingPool { get; } = "Strength + Athletics (if resisted)";
    public override string Effect { get; } = "Achieve flight";
    public override string? AdditionalNotes { get; } = "Can move at running speed and carrying a human-sized mass slows to walking speed. This can only be used by the alchemist.";
    public override string Source { get; } = "Corebook, page 287";
}