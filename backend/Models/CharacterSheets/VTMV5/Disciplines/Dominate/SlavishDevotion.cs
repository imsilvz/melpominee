namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class SlavishDevotion : VampireDiscipline
{
    public override string Id { get; } = "slavish_devotion";
    public override string Name { get; } = "Slavish Devotion";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 2;
    public override string? Prerequisite { get; } = "Mithras";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 1,
        School = "Presence",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Those already underneath Dominate find it easier to resist other kindred's Dominate.";
    public override string? AdditionalNotes { get; } = "Difficulty cannot rise above 7.";
    public override string Source { get; } = "Cults of the Blood Gods, page 104";
}