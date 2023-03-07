namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanLasombra : VampireClan
{
    public override string Id { get; } = "lasombra";
    public override string Name { get; } = "Lasombra";
    public override string Bane { get; } = "Distorted Image - In reflections or recordings (live or not) the Lasombra appear to be distorted, those who know what vampires are know precisely what's going on, while others might be confused but know something is wrong. This does not however, hide their identity with any certainty and they are not likely to be caught more often on surveillance than any other Kindred. In addition to this, modern communication technology which includes making a phone call requires a Technology test at Difficulty 2 + Bane Severity as microphones struggle with them as much as cameras. Avoiding any electronic vampire detection system is also done with a penalty equal to their Bane Severity.";
    public override List<string> Disciplines { get; } = new List<string> { "Dominate", "Oblivion", "Potence" };
}