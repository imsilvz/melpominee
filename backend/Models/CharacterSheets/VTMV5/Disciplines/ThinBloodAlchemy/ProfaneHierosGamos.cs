namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class ProfaneHierosGamos : VampirePower
{
    public override string Id { get; } = "profane_hieros_gamos";
    public override string Name { get; } = "Profane Hieros Gamos";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 3;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Melancholic, Phlegmatic",
    };
    public override string Cost { get; } = "One Rouse Check or One point of Aggravated Damage for mortals";
    public override string Duration { get; } = "Permanent.";
    public override string DicePool { get; } = "Stamina + Resolve";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Change their form into their ideal human shape.";
    public override string? AdditionalNotes { get; } = "This cannot spare the Nosferatu from their bane. [Errata]";
    public override string Source { get; } = "Corebook, page 286";
}