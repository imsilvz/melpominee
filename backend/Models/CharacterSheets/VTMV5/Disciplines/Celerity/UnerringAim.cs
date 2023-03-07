namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Celerity;

public class UnerringAim : VampireDiscipline
{
    public override string Id { get; } = "unerring_aim";
    public override string Name { get; } = "Unerring Aim";
    public override string School { get; } = "Celerity";
    public override int Level { get; } = 4;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Auspex",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "A single attack";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "The world slows down to a crawl";
    public override string? AdditionalNotes { get; } = "Those with Celerity 5 may nullify this power with a Rouse Check and defend.";
    public override string Source { get; } = "Corebook, page 254";
}