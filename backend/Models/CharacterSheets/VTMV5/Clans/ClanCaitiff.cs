namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanCaitiff : VampireClan
{
    public override string Id { get; } = "caitiff";
    public override string Name { get; } = "Caitiff";
    public override string Bane { get; } = "Untouched by their ancestors, the Caitiff do not share a common Bane. The character begins with the Flaw Suspect (•) and they may not purchase positive status during Character Creation. The Storyteller may impose a 1-2 dice penalty to social tests against Kindred who know they are Caitiff. To improve a Discipline, the cost is 6 time the level in experience points.";
    public override List<string> Disciplines { get; } = new List<string> { };
}