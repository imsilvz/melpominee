namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class ShadowCloak : VampirePower
{
    public override string Id { get; } = "shadow_cloak";
    public override string Name { get; } = "Shadow Cloak";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "+2 bonus to stealth rolls and intimidation against mortals.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Chicago by Night, page 293";
}