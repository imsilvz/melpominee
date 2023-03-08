namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class ClearTheField : VampirePower
{
    public override string Id { get; } = "clear_the_field";
    public override string Name { get; } = "Clear the Field";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 3,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "Composure + Presence";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Clear a space in a calm and orderly manner.";
    public override string? AdditionalNotes { get; } = "Opposed individually with Wits + Composure";
    public override string Source { get; } = "Fall of London, page 177";
}