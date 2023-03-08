namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class VisceralAbsorption : VampirePower
{
    public override string Id { get; } = "visceral_absorption";
    public override string Name { get; } = "Visceral Absorption";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Blood Sorcery ●●",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One turn per body";
    public override string DicePool { get; } = "Strength + Protean";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Draw in the remains of blood and body to the vampire to clean a scene.";
    public override string? AdditionalNotes { get; } = "Hunger can be reduced by one per body, up to the level of their Blood Sorcery rating but cannot reduce to 0.";
    public override string Source { get; } = "Sabbat, page 49";
}