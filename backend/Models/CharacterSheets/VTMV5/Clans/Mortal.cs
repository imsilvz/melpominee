namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class Mortal : VampireClan
{
    public override string Id { get; } = "";
    public override string Name { get; } = "Mortal";
    public override string Bane { get; } = "Mortals have no vampiric curses!";
    public override List<string> Disciplines { get; } = new List<string> { };
}