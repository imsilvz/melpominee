namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class FarReach : VampireDiscipline
{
    public override string Id { get; } = "far_reach";
    public override string Name { get; } = "Far Reach";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 1;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Choleric",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn unless held";
    public override string DicePool { get; } = "Resolve + Alchemy";
    public override string OpposingPool { get; } = "Strength + Athletics";
    public override string Effect { get; } = "Push, pull, hold or grab objects or people with their mind.";
    public override string? AdditionalNotes { get; } = "Keeping something held in the air requires a check each turn of Resolve + Alchemy Difficulty 3.";
    public override string Source { get; } = "Corebook, page 284";
}