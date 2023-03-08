namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class Obdurate : VampirePower
{
    public override string Id { get; } = "obdurate";
    public override string Name { get; } = "Obdurate";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Potence",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Wits + Survival";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Maintain footing when hit with a massive force.";
    public override string? AdditionalNotes { get; } = "Any superficial damage from falling or being hit is reduced by the Fortitude score, before being halved.";
    public override string Source { get; } = "Winter's Teeth #3";
}