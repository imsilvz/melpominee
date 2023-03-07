namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class UnburdeningTheBestialSoul : VampireDiscipline
{
    public override string Id { get; } = "unburdening_the_bestial_soul";
    public override string Name { get; } = "Unburdening the Bestial Soul";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 5;
    public override string? Prerequisite { get; } = "Obeah";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 3,
        School = "Dominate",
    };
    public override string Cost { get; } = "Two Rouse checks, 1 Stain";
    public override string Duration { get; } = "One session";
    public override string DicePool { get; } = "Composure + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Stain removal or protection from Stains";
    public override string? AdditionalNotes { get; } = "This power only works on vampires with a lower Humanity rating.";
    public override string Source { get; } = "Companion, page 24";
}