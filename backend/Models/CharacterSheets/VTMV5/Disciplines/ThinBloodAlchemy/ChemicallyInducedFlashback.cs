namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class ChemicallyInducedFlashback : VampirePower
{
    public override string Id { get; } = "chemicallyinduced_flashback";
    public override string Name { get; } = "Chemically-Induced Flashback";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Ashfinders, Concoct Ashe";
    public override string Cost { get; } = "One Rouse Check in addition to the use of Ashe";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Imbue and experience a Memorium of another vampire.";
    public override string? AdditionalNotes { get; } = "Must use the Fixatio method. The Thin-bloods develop this formula within the Ashfinders.";
    public override string Source { get; } = "Cults of the Blood Gods, page 45";
}