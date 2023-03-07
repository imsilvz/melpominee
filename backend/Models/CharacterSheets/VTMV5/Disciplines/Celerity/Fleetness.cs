namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class Fleetness : VampireDiscipline
{
    public override string Id { get; } = "fleetness";
    public override string Name { get; } = "Fleetness";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Add Celerity rating for non-combat Dexterity test or defending.";
    public override string? AdditionalNotes { get; } = "This may be used once per turn when defending with associated pools.";
    public override string Source { get; } = "Corebook, page 253";
}