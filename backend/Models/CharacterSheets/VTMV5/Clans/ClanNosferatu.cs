namespace Melpominee.app.Models.CharacterSheets.VTMV5.Clans;

public class ClanNosferatu : VampireClan
{
    public override string Id { get; } = "nosferatu";
    public override string Name { get; } = "Nosferatu";
    public override string Bane { get; } = "Repulsiveness - Cursed by their blood, when they are Embraced they are twisted into revolting monsters. They can never raise their rating in the Looks merits and instead must take the (••) Repulsive flaw. Any attempt to disguise themselves incurs a penalty equal to the character's Bane Severity, this also includes the use of Disciplines such as Mask of a Thousand Faces. However, most Nosferatu do not breach the Masquerade by being seen, they are instead perceived as gross or terrifying.";
    public override List<string> Disciplines { get; } = new List<string> { "Animalism", "Obfuscate", "Potence" };
}