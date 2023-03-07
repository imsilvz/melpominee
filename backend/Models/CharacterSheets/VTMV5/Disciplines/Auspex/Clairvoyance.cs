namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class Clairvoyance : VampireDiscipline
{
    public override string Id { get; } = "clairvoyance";
    public override string Name { get; } = "Clairvoyance";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Few minutes up to one night";
    public override string DicePool { get; } = "Intelligence + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Information gathering from surroundings";
    public override string? AdditionalNotes { get; } = "This power can be used to monitor events in progress.";
    public override string Source { get; } = "Corebook, page 251";
}