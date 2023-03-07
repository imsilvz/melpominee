namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class SealTheBeastsMaw : VampireDiscipline
{
    public override string Id { get; } = "seal_the_beasts_maw";
    public override string Name { get; } = "Seal the Beast's Maw";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 3;
    public override string? Prerequisite { get; } = "Eremites";
    public override string Cost { get; } = "Two Rouse Checks";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "A vampire can ignore the effects of hunger if they do not increase hunger from the two Rouse Checks, but reduce their dice pools.";
    public override string? AdditionalNotes { get; } = "If a dice pool reduces to 0, a Fury Frenzy test is made.";
    public override string Source { get; } = "Forbidden Religions, page 44";
}