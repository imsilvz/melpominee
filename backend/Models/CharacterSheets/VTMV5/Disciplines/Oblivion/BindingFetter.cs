namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class BindingFetter : VampireDiscipline
{
    public override string Id { get; } = "binding_fetter";
    public override string Name { get; } = "Binding Fetter";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Allow users to be able to identify a fetter by use of their senses.";
    public override string? AdditionalNotes { get; } = "During its use the user receives a -2 penalty to all Awareness, Wits, and Resolve rolls.";
    public override string Source { get; } = "Cults of the Blood Gods, page 204";
}