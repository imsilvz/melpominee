namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class DraughtOfElegance : VampirePower
{
    public override string Id { get; } = "draught_of_elegance";
    public override string Name { get; } = "Draught of Elegance";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Night; for drinkers until the vampire's next feeding or Hunger 5";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn their vitae into a Celerity boost for others";
    public override string? AdditionalNotes { get; } = "Each drinker must take one Rouse Checks worth.";
    public override string Source { get; } = "Corebook, page 254";
}