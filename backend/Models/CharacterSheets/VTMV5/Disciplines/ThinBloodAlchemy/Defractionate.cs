namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class Defractionate : VampirePower
{
    public override string Id { get; } = "defractionate";
    public override string Name { get; } = "Defractionate";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Melancholic, Sanguine",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn preserved blood into something palatable by any kindred.";
    public override string? AdditionalNotes { get; } = "Each Distillation style has a unique method of tapping for this elixir.";
    public override string Source { get; } = "Corebook, page 286";
}