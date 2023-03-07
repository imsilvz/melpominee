namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class FatalPrecognition : VampireDiscipline
{
    public override string Id { get; } = "fatal_precognition";
    public override string Name { get; } = "Fatal Precognition";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 2;
    public override VampireDisciplineAmalgam? Amalgam { get; } = new VampireDisciplineAmalgam
    {
        Level = 2,
        School = "Auspex",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "Until fulfilled, avoided or the story ends";
    public override string DicePool { get; } = "Resolve + Oblivion";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Vision of a non-vampires death.";
    public override string? AdditionalNotes { get; } = "The vampire must be able to see or hear the target during the power's use.";
    public override string Source { get; } = "Cults of the Blood Gods, page 204";
}