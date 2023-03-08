namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class PassionFeast : VampirePower
{
    public override string Id { get; } = "passion_feast";
    public override string Name { get; } = "Passion Feast";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Fortitude",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "Resolve + Oblivion";
    public override string OpposingPool { get; } = "Resolve + Composure";
    public override string Effect { get; } = "Allows a vampire to slake Hunger on the passion of wraiths.";
    public override string? AdditionalNotes { get; } = "Feeding from wraiths may merit a Stain at the Storytellers discretion.";
    public override string Source { get; } = "Cults of the Blood Gods, page 206";
}