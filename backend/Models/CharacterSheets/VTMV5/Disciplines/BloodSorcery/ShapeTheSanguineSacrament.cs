namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class ShapeTheSanguineSacrament : VampirePower
{
    public override string Id { get; } = "shape_the_sanguine_sacrament";
    public override string Name { get; } = "Shape the Sanguine Sacrament";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "One Scene unless deactivated";
    public override string DicePool { get; } = "Manipulation + Blood Sorcery";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Shape blood into a shape or image.";
    public override string? AdditionalNotes { get; } = "If the user uses their vitae, it costs one Rouse Check.";
    public override string Source { get; } = "Winter's Teeth #3";
}