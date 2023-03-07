namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanThinBlood : VampireClan
{
    public override string Id { get; } = "thin_blood";
    public override string Name { get; } = "Thin-Blood";
    public override string Bane { get; } = "Thin-bloods do not suffer from a Bane unless the thin-blood Flaw Clan Curse is taken.";
    public override List<string> Disciplines { get; } = new List<string> { "Thin-blood Alchemy" };
}