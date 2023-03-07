namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanMalkavian : VampireClan
{
    public override string Id { get; } = "malkavian";
    public override string Name { get; } = "Malkavian";
    public override string Bane { get; } = "Fractured Perspective - When suffering a Bestial Failure or a Compulsion, their mental derangement comes to the forefront. Suffers a penalty equal to their Bane Severity to one category of dice pools (Physical, Social or Mental) for the entire scene. The penalty and nature of the affliction are decided between the player and Storyteller during character creation.";
    public override List<string> Disciplines { get; } = new List<string> { "Auspex", "Dominate", "Obfuscate" };
}