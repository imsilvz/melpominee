namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class EyesOfTheSerpent : VampirePower
{
    public override string Id { get; } = "eyes_of_the_serpent";
    public override string Name { get; } = "Eyes of the Serpent";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 1;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Protean",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Until eye contact is broken or the scene ends";
    public override string DicePool { get; } = "Charisma + Presence";
    public override string OpposingPool { get; } = "Wits + Composure";
    public override string Effect { get; } = "Immobilize a victim by making eye contact.";
    public override string? AdditionalNotes { get; } = "A vampire victim can break this by spending Willpower any turn after the first.";
    public override string Source { get; } = "Anarch, page 185";
}