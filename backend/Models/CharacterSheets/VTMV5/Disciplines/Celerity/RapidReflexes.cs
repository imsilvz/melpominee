namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class RapidReflexes : VampireDiscipline
{
    public override string Id { get; } = "rapid_reflexes";
    public override string Name { get; } = "Rapid Reflexes";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 1;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Passive";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Faster reactions and minor actions.";
    public override string? AdditionalNotes { get; } = "This power also prevents them from taking a penalty when they have no cover during a firefight.";
    public override string Source { get; } = "Corebook, page 253";
}