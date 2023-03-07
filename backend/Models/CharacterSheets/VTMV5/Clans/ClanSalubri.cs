namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanSalubri : VampireClan
{
    public override string Id { get; } = "salubri";
    public override string Name { get; } = "Salubri";
    public override string Bane { get; } = "Hunted - Their vitae has a unique trait where when another clan partakes in their Blood they find it difficult to pull away. Once a non-Salubri has consumed at least one Hunger level worth, they must make a Hunger Frenzy test at difficulty 2 + the Salubri's Bane Severity (3 + the Salubri's Bane Severity for Banu Haqim). If they fail, they will continue to consume the Salubri until pried off. Additionally, each Salubri has a third eye and while it's not always human-like it's always present and cannot be obscured by supernatural powers. In addition to this, whenever they activate a Discipline, the eye weeps vitae with its intensity correlating to the level of the Discipline used. The Blood flowing from the eye can trigger a Hunger Frenzy test from nearby vampires with Hunger 4 or more.";
    public override List<string> Disciplines { get; } = new List<string> { "Auspex", "Dominate", "Fortitude" };
}