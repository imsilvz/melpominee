namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Potence;

public class SparkOfRage : VampirePower
{
    public override string Id { get; } = "spark_of_rage";
    public override string Name { get; } = "Spark of Rage";
    public override string School { get; } = "Potence";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 3,
        School = "Presence",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user can add their Potence rating to rile or incite a person or crowd to violent actions.";
    public override string? AdditionalNotes { get; } = "Against vampires, the user will roll Manipulation + Potence vs Intelligence + Composure.";
    public override string Source { get; } = "Corebook, page 265";
}