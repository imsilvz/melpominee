namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanTremere : VampireClan
{
    public override string Id { get; } = "tremere";
    public override string Name { get; } = "Tremere";
    public override string Bane { get; } = "Deficient Blood - Before the fall of Vienna, they were defined by their rigid hierarchy of Blood Bonds within the Pyramid. After the fall, their Blood weakened and rejected all prior connections. Tremere are unable to Blood Bond other Kindred, though they can still be Bound by other clans. Tremere can still Blood Bond mortals to do their bidding, but the vitae must drink an additional amount of times equal to their Bane Severity.";
    public override List<string> Disciplines { get; } = new List<string> { "Auspex", "Dominate", "Blood Sorcery" };
}