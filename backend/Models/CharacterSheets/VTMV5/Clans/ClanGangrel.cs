namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanGangrel : VampireClan
{
    public override string Id { get; } = "gangrel";
    public override string Name { get; } = "Gangrel";
    public override string Bane { get; } = "Bestial Features - In Frenzy, Gangrel gains animalistic features equal to their Bane Severity. These features last for one more night afterward, each feature reducing one Attribute by 1 point. The Gangrel may choose to Ride the Wave in order to only have one feature manifest and only lose one Attribute point.";
    public override List<string> Disciplines { get; } = new List<string> { "Animalism", "Fortitude", "Protean" };
}