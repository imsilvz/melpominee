namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class AwakenTheSleeper : VampirePower
{
    public override string Id { get; } = "awaken_the_sleeper";
    public override string Name { get; } = "Awaken the Sleeper";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 5;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Choleric or Sanguine",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "N/A";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Awaken a vampire from torpor";
    public override string? AdditionalNotes { get; } = "Each Distillation style has a unique method of tapping for this elixir.";
    public override string Source { get; } = "Corebook, page 287";
}