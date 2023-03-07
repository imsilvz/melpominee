namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class CorrosiveVitae : VampireDiscipline
{
    public override string Id { get; } = "corrosive_vitae";
    public override string Name { get; } = "Corrosive Vitae";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "One or more Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn vitae corrosive.";
    public override string? AdditionalNotes { get; } = "Does not work against unliving flesh, such as other vampires.";
    public override string Source { get; } = "Corebook, page 272";
}