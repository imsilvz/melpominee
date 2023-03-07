namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class Ventriloquism : VampireDiscipline
{
    public override string Id { get; } = "ventriloquism";
    public override string Name { get; } = "Ventriloquism";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Auspex",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Wits + Obfuscate";
    public override string OpposingPool { get; } = "Resolve + Composure";
    public override string Effect { get; } = "Throw their voice so only the intended recipient can hear it.";
    public override string? AdditionalNotes { get; } = "Can be used on anyone in line of sight.";
    public override string Source { get; } = "Fall of London, page 148";
}