namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class FangStinger : VampirePower
{
    public override string Id { get; } = "fangstinger";
    public override string Name { get; } = "Fang-Stinger";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Second Inquisition";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Choleric",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One day";
    public override string DicePool { get; } = "Resolve + Alchemy";
    public override string OpposingPool { get; } = "Stamina + Resolve";
    public override string Effect { get; } = "Introduce blood into a mortal that causes no harm to them but harms vampire's that feed from them.";
    public override string? AdditionalNotes { get; } = "Thin-bloods develop this formula within the Second Inquisition.";
    public override string Source { get; } = "Second Inquisition, page 47";
}