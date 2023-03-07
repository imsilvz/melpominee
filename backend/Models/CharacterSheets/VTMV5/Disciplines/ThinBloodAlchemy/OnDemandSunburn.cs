namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class OnDemandSunburn : VampireDiscipline
{
    public override string Id { get; } = "ondemand_sunburn";
    public override string Name { get; } = "On-Demand Sunburn";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Sabbat";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Choleric",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Until unleashed or next sunset, whichever is first";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Become a sun battery and harm vampires they touch.";
    public override string? AdditionalNotes { get; } = "This formula was developed by the Path of the Sun Thin-bloods.";
    public override string Source { get; } = "Sabbat, page 53";
}