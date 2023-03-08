namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class ReclamationOfVitae : VampirePower
{
    public override string Id { get; } = "reclamation_of_vitae";
    public override string Name { get; } = "Reclamation of Vitae";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One or more Stains";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Reclaim the Blood given to create ghouls over distance.";
    public override string? AdditionalNotes { get; } = "Those outside of the Sabbat take stains upon use.";
    public override string Source { get; } = "Sabbat, page 50";
}