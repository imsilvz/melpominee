namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class StayTheFallingSand : VampireDiscipline
{
    public override string Id { get; } = "stay_the_falling_sand";
    public override string Name { get; } = "Stay the Falling Sand";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Melancholic",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One round or more if maintained, expires after one use";
    public override string DicePool { get; } = "Resolve + Alchemy";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Slow time";
    public override string? AdditionalNotes { get; } = "Cannot be used on living creatures or kindred.";
    public override string Source { get; } = "Winter's Teeth, Page 10";
}