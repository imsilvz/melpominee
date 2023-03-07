namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class BloodOfMandagloire : VampireDiscipline
{
    public override string Id { get; } = "blood_of_mandagloire";
    public override string Name { get; } = "Blood of Mandagloire";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 2;
    public override string? Prerequisite { get; } = "Second Inquisition";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = null,
        School = "Melancholic",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Remains active in the herd's blood for three nights";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Infect the blood of a herd to cause those who feed from it to fall into a dreamless sleep.";
    public override string? AdditionalNotes { get; } = "Thin-bloods develop this formula within the Second Inquisition.";
    public override string Source { get; } = "Second Inquisition, 46";
}