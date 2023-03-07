namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanHecata : VampireClan
{
    public override string Id { get; } = "hecata";
    public override string Name { get; } = "Hecata";
    public override string Bane { get; } = "Painful Kiss - Hecata may only take harmful drinks from mortals which result in blood loss. Unwilling mortals that are able to escape will make the attempt, even those who are convinced or willing must succeed in a Stamina + Resolve test against Difficulty 2 + Bane Severity in order to not recoil. Vampires who are willingly bit must make a Frenzy test against Difficulty 3 to avoid terror Frenzy.";
    public override List<string> Disciplines { get; } = new List<string> { "Auspex", "Fortitude", "Oblivion" };
}