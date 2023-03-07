namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class LightningStrike : VampireDiscipline
{
    public override string Id { get; } = "lightning_strike";
    public override string Name { get; } = "Lightning Strike";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "A single attack";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Attack with lightning speed";
    public override string? AdditionalNotes { get; } = "Those with Celerity 5 may nullify this power with a Rouse Check and defend.";
    public override string Source { get; } = "Corebook, page 254";
}