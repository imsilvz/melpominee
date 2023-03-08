namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class FreezerFluid : VampirePower
{
    public override string Id { get; } = "freezer_fluid";
    public override string Name { get; } = "Freezer Fluid";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Second Inquisition";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Melancholic, Phlegmatic",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Resolve + Alchemy";
    public override string OpposingPool { get; } = "Stamina + Resolve";
    public override string Effect { get; } = "Freeze a vampire's body.";
    public override string? AdditionalNotes { get; } = "Thin-bloods develop this formula within the Second Inquisition.";
    public override string Source { get; } = "Second Inquisition, page 47";
}