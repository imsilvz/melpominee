namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class EyesOfBeasts : VampirePower
{
    public override string Id { get; } = "eyes_of_beasts";
    public override string Name { get; } = "Eyes of Beasts";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Animalism",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Share the senses with animals.";
    public override string? AdditionalNotes { get; } = "This power works through groups of animals.";
    public override string Source { get; } = "Fall of London, page 148";
}