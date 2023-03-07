namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Animalism;

public class QuellTheBeast : VampireDiscipline
{
    public override string Id { get; } = "quell_the_beast";
    public override string Name { get; } = "Quell the Beast";
    public override string School { get; } = "Animalism";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Charisma + Animalism";
    public override string OpposingPool { get; } = "Stamina + Animalism";
    public override string Effect { get; } = "Force a vampire's beast to slumber or make a mortal lethargic.";
    public override string? AdditionalNotes { get; } = "Against Vampires this lasts several turns equal to test margin +1.";
    public override string Source { get; } = "Corebook, page 246";
}