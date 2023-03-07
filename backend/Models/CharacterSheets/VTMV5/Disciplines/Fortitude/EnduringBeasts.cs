namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class EnduringBeasts : VampireDiscipline
{
    public override string Id { get; } = "enduring_beasts";
    public override string Name { get; } = "Enduring Beasts";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 1,
        School = "Animalism",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Stamina + Animalism";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Share the vampire's toughness with animals.";
    public override string? AdditionalNotes { get; } = "If used on their Famulus, it is free and automatic without a roll required.";
    public override string Source { get; } = "Corebook, page 258";
}