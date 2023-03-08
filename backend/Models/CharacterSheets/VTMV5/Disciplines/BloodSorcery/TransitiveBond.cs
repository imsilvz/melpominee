namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class TransitiveBond : VampirePower
{
    public override string Id { get; } = "transitive_bond";
    public override string Name { get; } = "Transitive Bond";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Extend the properties of Blood Bonding in vitae.";
    public override string? AdditionalNotes { get; } = "This power was developed originally by the Tremere to combat their bane to only resurface within the Sabbat.";
    public override string Source { get; } = "Sabbat, page 49";
}