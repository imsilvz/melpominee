namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanMinistry : VampireClan
{
    public override string Id { get; } = "ministry";
    public override string Name { get; } = "The Ministry";
    public override string Bane { get; } = "Abhors the Light - When a Minister is exposed to direct illumination, be it naturally caused or artificial, they receive a penalty equal to their Bane Severity to all dice pools while subject to the light. They also add their Bane Severity to Aggravated damage taken from sunlight.";
    public override List<string> Disciplines { get; } = new List<string> { "Protean", "Obfuscate", "Presence" };
}