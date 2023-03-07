namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class FataMorgana : VampireDiscipline
{
    public override string Id { get; } = "fata_morgana";
    public override string Name { get; } = "Fata Morgana";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 3;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Presence",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene, unless let to lapse";
    public override string DicePool { get; } = "Manipulation + Obfuscate";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Elaborate hallucinations.";
    public override string? AdditionalNotes { get; } = "These hallucinations cannot cause damage or appear to affect the surrounding reality. They cannot blind or deafen through sensory overloading.";
    public override string Source { get; } = "Companion, page 26";
}