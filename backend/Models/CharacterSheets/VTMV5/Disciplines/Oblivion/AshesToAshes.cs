namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class AshesToAshes : VampireDiscipline
{
    public override string Id { get; } = "ashes_to_ashes";
    public override string Name { get; } = "Ashes to Ashes";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Variable";
    public override string DicePool { get; } = "Stamina + Oblivion";
    public override string OpposingPool { get; } = "Stamina + Medicine/Fortitude";
    public override string Effect { get; } = "Destroy a corpse by dissolving it.";
    public override string? AdditionalNotes { get; } = "If the body is not animated it will dissolve throughout three turns with no test needed.";
    public override string Source { get; } = "Cults of the Blood Gods, page 204";
}