namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class BondFamulus : VampireDiscipline
{
    public override string Id { get; } = "bond_famulus";
    public override string Name { get; } = "Bond Famulus";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Only death releases the famulus.";
    public override string DicePool { get; } = "Charisma + Animal Ken";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Create an enhanced animal companion.";
    public override string? AdditionalNotes { get; } = "Vampires may only have one famulus but you can make animal ghouls without this.";
    public override string Source { get; } = "Corebook, page 245";
}