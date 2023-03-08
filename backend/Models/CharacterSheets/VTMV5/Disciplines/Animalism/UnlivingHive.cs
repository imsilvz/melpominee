namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class UnlivingHive : VampirePower
{
    public override string Id { get; } = "unliving_hive";
    public override string Name { get; } = "Unliving Hive";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Obfuscate",
    };
    public override string Cost { get; } = "None";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Extends Animalism influence to swarms of insects.";
    public override string? AdditionalNotes { get; } = "Swarms are treated as single creatures.";
    public override string Source { get; } = "Corebook, page 246";
}