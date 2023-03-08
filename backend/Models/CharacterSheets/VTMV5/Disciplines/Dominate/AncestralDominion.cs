namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Dominate;

public class AncestralDominion : VampirePower
{
    public override string Id { get; } = "ancestral_dominion";
    public override string Name { get; } = "Ancestral Dominion";
    public override string School { get; } = "Dominate";
    public override int Level { get; } = 4;
    public override string? Prerequisite { get; } = "Mithras";
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Blood Sorcery ●●●",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene or when the directive is fulfilled";
    public override string DicePool { get; } = "Manipulation + Dominate";
    public override string OpposingPool { get; } = "Intelligence + Resolve";
    public override string Effect { get; } = "Compel a descendant to act even if against their own opinion.";
    public override string? AdditionalNotes { get; } = "For each generation after the first separating them, the target gains an additional die to resist.";
    public override string Source { get; } = "Cults of the Blood Gods, page 104";
}