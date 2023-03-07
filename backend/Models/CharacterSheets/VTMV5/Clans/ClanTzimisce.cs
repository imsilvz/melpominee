namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanTzimisce : VampireClan
{
    public override string Id { get; } = "tzimisce";
    public override string Name { get; } = "Tzimisce";
    public override string Bane { get; } = "Grounded - Each Tzimisce must select a specific charge, be it physical location, a group of people, or something even more esoteric. Each night they must sleep surrounded by their charge, if they do not, they sustain aggravated Willpower damage equal to their Bane Severity upon waking the following night.";
    public override List<string> Disciplines { get; } = new List<string> { "Animalism", "Dominate", "Protean" };
}