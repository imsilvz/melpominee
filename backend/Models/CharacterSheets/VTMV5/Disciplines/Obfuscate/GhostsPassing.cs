namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class GhostsPassing : VampireDiscipline
{
    public override string Id { get; } = "ghosts_passing";
    public override string Name { get; } = "Ghost's Passing";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 1,
        School = "Animalism",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One session";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The user can bestow Obfuscate onto an animal.";
    public override string? AdditionalNotes { get; } = "Sense the Unseen can discern signs as per general Obfuscate rules";
    public override string Source { get; } = "Forbidden Religions, page 18";
}