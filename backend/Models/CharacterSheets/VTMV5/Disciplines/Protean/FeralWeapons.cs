namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class FeralWeapons : VampirePower
{
    public override string Id { get; } = "feral_weapons";
    public override string Name { get; } = "Feral Weapons";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 2;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Elongate the user's nails into claws, gaining a +2 modifier to damage or elongate their fangs and not suffer a called shot penalty.";
    public override string? AdditionalNotes { get; } = "Superficial damage inflicted by the user is not halved while active.";
    public override string Source { get; } = "Corebook, page 270";
}