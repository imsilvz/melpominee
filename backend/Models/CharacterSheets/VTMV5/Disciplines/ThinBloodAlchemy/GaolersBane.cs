namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class GaolersBane : VampirePower
{
    public override string Id { get; } = "gaolers_bane";
    public override string Name { get; } = "Gaoler's Bane";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 1;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Sanguine",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One Scene or until ended voluntarily";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Receive a 2-dice bonus when freeing themselves from physical restraints or grapples.";
    public override string? AdditionalNotes { get; } = "N/A";
    public override string Source { get; } = "Winter's Teeth, Page 10";
}