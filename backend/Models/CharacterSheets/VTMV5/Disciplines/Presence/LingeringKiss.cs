namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class LingeringKiss : VampireDiscipline
{
    public override string Id { get; } = "lingering_kiss";
    public override string Name { get; } = "Lingering Kiss";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "Free";
    public override string Duration { get; } = "Number of nights equal to the user's Presence rating";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Usable during feedings, the victim gains a bonus to Social attribute for one night.";
    public override string? AdditionalNotes { get; } = "A withdrawal follows where the victim takes a penalty equal to the original bonus when not actively working towards the next fix. It cannot be used on those under a Blood Bond. Unbondable cannot take this discipline power. [Errata]";
    public override string Source { get; } = "Corebook, page 267 Companion page 62";
}