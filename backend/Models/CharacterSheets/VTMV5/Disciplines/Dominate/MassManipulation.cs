namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class MassManipulation : VampireDiscipline
{
    public override string Id { get; } = "mass_manipulation";
    public override string Name { get; } = "Mass Manipulation";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check in addition to power it's added to";
    public override string Duration { get; } = "As per power amplified";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Extend effects of Dominate to multiple targets.";
    public override string? AdditionalNotes { get; } = "The victims need to see the eyes of the user. The user makes the roll against the strongest of the group.";
    public override string Source { get; } = "Corebook, page 257";
}