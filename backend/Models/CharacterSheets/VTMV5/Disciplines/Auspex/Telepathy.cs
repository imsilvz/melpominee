namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class Telepathy : VampirePower
{
    public override string Id { get; } = "telepathy";
    public override string Name { get; } = "Telepathy";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse (one Willpower on non-consenting vampires)";
    public override string Duration { get; } = "One minute per rouse or full scene on consenting subject";
    public override string DicePool { get; } = "Resolve + Auspex";
    public override string OpposingPool { get; } = "Wits + Subterfuge";
    public override string Effect { get; } = "Read minds and project thoughts";
    public override string? AdditionalNotes { get; } = "The user doesn't need to roll to project their thoughts onto others.";
    public override string Source { get; } = "Corebook, page 252";
}