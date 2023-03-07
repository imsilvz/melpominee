namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class CloakOfShadows : VampireDiscipline
{
    public override string Id { get; } = "cloak_of_shadows";
    public override string Name { get; } = "Cloak of Shadows";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "As long as the user stands still they blend into their surroundings.";
    public override string? AdditionalNotes { get; } = "Follows the general rules for Obfuscate.";
    public override string Source { get; } = "Corebook, page 261";
}