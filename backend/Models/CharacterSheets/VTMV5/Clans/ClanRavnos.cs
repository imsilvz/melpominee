namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanRavnos : VampireClan
{
    public override string Id { get; } = "ravnos";
    public override string Name { get; } = "Ravnos";
    public override string Bane { get; } = "Doomed - Anytime they daysleep within the same place more than once in seven nights, roll a number of dice equal to their Bane Severity. If they receive any 10's they then take 1 Aggravated damage for each as they are scorched from within. What constitutes as the same place is defined by the chronicle, but generally will need a mile distance between the two resting places before the bane is triggered. Mobile havens do work as long as the haven is moved a mile away. Due to this, the Ravnos may not take the No Haven Flaw.";
    public override List<string> Disciplines { get; } = new List<string> { "Animalism", "Obfuscate", "Presence" };
}