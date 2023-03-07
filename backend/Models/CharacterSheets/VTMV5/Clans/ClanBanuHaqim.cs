namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanBanuHaqim : VampireClan
{
    public override string Id { get; } = "banu_haqim";
    public override string Name { get; } = "Banu Haqim";
    public override string Bane { get; } = "Blood Addiction - When the Banu Haqim slakes at least one Hunger level from another vampire, they must make a Hungry Frenzy test at difficulty 2 plus Bane Severity. If they fail, they must gorge themselves on vitae, in turn opening the door to possible Diablerie.";
    public override List<string> Disciplines { get; } = new List<string> { "Blood Sorcery", "Celerity", "Obfuscate" };
}