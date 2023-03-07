namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanVentrue : VampireClan
{
    public override string Id { get; } = "ventrue";
    public override string Name { get; } = "Ventrue";
    public override string Bane { get; } = "Rarefied Tastes - When the Ventrue drinks the blood of a mortal who does not fall within their preference, they must spend Willpower equal to their Bane Severity else they will vomit the blood from their bodies unable to slake their hunger. Their preferences range within the clan, some looking for descendants of a certain nationality to soldiers suffering from PTSD. With a Resolve + Awareness test, they can sense if a mortal they seek to feed from fits within their preference. At character creation, their preference should be selected.";
    public override List<string> Disciplines { get; } = new List<string> { "Dominate", "Fortitude", "Presence" };
}