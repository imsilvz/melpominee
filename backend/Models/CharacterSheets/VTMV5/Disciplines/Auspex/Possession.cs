namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class Possession : VampireDiscipline
{
    public override string Id { get; } = "possession";
    public override string Name { get; } = "Possession";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 5;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 3,
        School = "Dominate",
    };
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "Until ended";
    public override string DicePool { get; } = "Resolve + Auspex";
    public override string OpposingPool { get; } = "Resolve + Intelligence";
    public override string Effect { get; } = "Possess a mortal body";
    public override string? AdditionalNotes { get; } = "This power does not give the ability to read the target's mind, use their skills or impersonate them.";
    public override string Source { get; } = "Corebook, page 251";
}