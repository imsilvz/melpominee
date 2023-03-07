namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class TheHeartOfDarkness : VampireDiscipline
{
    public override string Id { get; } = "the_heart_of_darkness";
    public override string Name { get; } = "The Heart of Darkness";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 5;
    public override string? Prerequisite { get; } = "Church of Set";
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Fortitude",
    };
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Permanent or until the heart is destroyed or returned to the host's body";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Allows a vampire to remove their own heart and store it outside of their body.";
    public override string? AdditionalNotes { get; } = "If the heart is dealt aggravated damage equal to or greater than the user's health tracker, they fall into torpor. It can only be destroyed through fire or sunlight.";
    public override string Source { get; } = "Cults of the Blood Gods, page 85";
}