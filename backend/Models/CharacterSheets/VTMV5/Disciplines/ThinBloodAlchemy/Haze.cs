namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class Haze : VampirePower
{
    public override string Id { get; } = "haze";
    public override string Name { get; } = "Haze";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 1;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Phlegmatic",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Scene or until ended voluntarily";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Create a mist that follows the user, making it harder to shoot them or identify them.";
    public override string? AdditionalNotes { get; } = "This can be extended to encompass up to five people with an additional Rouse Check.";
    public override string Source { get; } = "Corebook, page 285";
}