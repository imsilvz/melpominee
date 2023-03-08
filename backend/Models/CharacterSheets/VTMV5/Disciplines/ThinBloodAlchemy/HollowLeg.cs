namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class HollowLeg : VampirePower
{
    public override string Id { get; } = "hollow_leg";
    public override string Name { get; } = "Hollow Leg";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 4;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Non-Resonant Human Blood",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One night or until the victim suffers a hunger Frenzy";
    public override string DicePool { get; } = "Intelligence + Alchemy";
    public override string OpposingPool { get; } = "Stamina + Composure";
    public override string Effect { get; } = "Poison another Kindred so that they are unable to slake hunger.";
    public override string? AdditionalNotes { get; } = "The victim must be tricked, coerced or forced into drinking this.";
    public override string Source { get; } = "Winter's Teeth, Page 10";
}