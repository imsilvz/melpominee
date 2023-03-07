namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class Envelop : VampireDiscipline
{
    public override string Id { get; } = "envelop";
    public override string Name { get; } = "Envelop";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Melancholic, Phlegmatic",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Scene or until ended voluntarily";
    public override string DicePool { get; } = "Wits + Alchemy";
    public override string OpposingPool { get; } = "Stamina + Survival";
    public override string Effect { get; } = "Create a mist that clings to a victim.";
    public override string? AdditionalNotes { get; } = "This can only be employed on single targets one at a time.";
    public override string Source { get; } = "Corebook, page 285";
}