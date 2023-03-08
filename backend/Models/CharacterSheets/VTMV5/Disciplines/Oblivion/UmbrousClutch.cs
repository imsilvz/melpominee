namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class UmbrousClutch : VampirePower
{
    public override string Id { get; } = "umbrous_clutch";
    public override string Name { get; } = "Umbrous Clutch";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check, One Stain";
    public override string Duration { get; } = "Instant";
    public override string DicePool { get; } = "Wits + Oblivion";
    public override string OpposingPool { get; } = "Dexterity + Wits";
    public override string Effect { get; } = "Using the victim's shadow they create a portal, dropping them into the user's arms.";
    public override string? AdditionalNotes { get; } = "An unprepared mortal will be terrified while a vampire must test for fury or fear Frenzy test at a Difficulty 4.";
    public override string Source { get; } = "Sabbat, page 49";
}