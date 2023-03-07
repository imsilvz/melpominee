namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanBrujah : VampireClan
{
    public override string Id { get; } = "brujah";
    public override string Name { get; } = "Brujah";
    public override string Bane { get; } = "Violent Temper - A rage is simmering in the back of the mind with a Brujah with the slightest provocation able to send them into a frenzied rage. Subtracts dice equal to the Bane Severity of the Brujah against Fury Frenzy.";
    public override List<string> Disciplines { get; } = new List<string> { "Celerity", "Potence", "Presence" };
}