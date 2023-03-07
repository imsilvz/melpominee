namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Presence;

public class Summon : VampireDiscipline
{
    public override string Id { get; } = "summon";
    public override string Name { get; } = "Summon";
    public override string School { get; } = "Presence";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One night";
    public override string DicePool { get; } = "Manipulation + Presence";
    public override string OpposingPool { get; } = "Composure + Intelligence";
    public override string Effect { get; } = "Call someone to them who has had certain Presence powers used on them or tasted the user's vitae.";
    public override string? AdditionalNotes { get; } = "The victim will not physically or financially harm themselves to reach the user.";
    public override string Source { get; } = "Corebook, page 268";
}