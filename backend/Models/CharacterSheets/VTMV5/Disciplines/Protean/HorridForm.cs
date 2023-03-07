namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class HorridForm : VampireDiscipline
{
    public override string Id { get; } = "horrid_form";
    public override string Name { get; } = "Horrid Form";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Vicissitude";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene unless voluntarily ended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Take on a monstrous shape.";
    public override string? AdditionalNotes { get; } = "Grants several changes equal to the user's Protean rating. Any critical scores are counted as messy criticals and Frenzy checks are made with +2 Difficulty.";
    public override string Source { get; } = "Companion, page 28";
}