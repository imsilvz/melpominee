namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class Conceal : VampirePower
{
    public override string Id { get; } = "conceal";
    public override string Name { get; } = "Conceal";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 4;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 3,
        School = "Auspex",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One night plus one additional night per margin of success";
    public override string DicePool { get; } = "Intelligence + Obfuscate";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Cloak an inanimate object.";
    public override string? AdditionalNotes { get; } = "This power cannot affect anything larger than a two-story house or anything moving on it's own.";
    public override string Source { get; } = "Corebook, page 262";
}