namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class Valeren : VampirePower
{
    public override string Id { get; } = "valeren";
    public override string Name { get; } = "Valeren";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Auspex",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Intelligence + Fortitude";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Mend an injured vampire.";
    public override string? AdditionalNotes { get; } = "A subject can be affected by the power only once a night.";
    public override string Source { get; } = "Companion, page 25";
}