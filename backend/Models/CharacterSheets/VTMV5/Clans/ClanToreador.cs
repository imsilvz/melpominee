namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanToreador : VampireClan
{
    public override string Id { get; } = "toreador";
    public override string Name { get; } = "Toreador";
    public override string Bane { get; } = "Aesthetic Fixation - A desire for beauty takes control over the Toreador and when in lesser surroundings they suffer. When they are within settings they find less than beautiful, they take a penalty equal to their Bane Severity when using Disciplines.";
    public override List<string> Disciplines { get; } = new List<string> { "Auspex", "Celerity", "Presence" };
}