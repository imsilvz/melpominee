namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class Obeah : VampirePower
{
    public override string Id { get; } = "obeah";
    public override string Name { get; } = "Obeah";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Fortitude",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Composure + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Heals Willpower and calms nerves.";
    public override string? AdditionalNotes { get; } = "If the user uses this on more than one subject on the same night, the cost increases to spend Willpower equal to half the number of successes in the margin per additional target.";
    public override string Source { get; } = "Companion, page 24";
}