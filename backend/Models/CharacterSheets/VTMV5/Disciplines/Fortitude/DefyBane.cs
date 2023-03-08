namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Fortitude;

public class DefyBane : VampirePower
{
    public override string Id { get; } = "defy_bane";
    public override string Name { get; } = "Defy Bane";
    public override string School { get; } = "Fortitude";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene or until expired";
    public override string DicePool { get; } = "Wits + Survival";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Convert Aggravated Damage to Superficial Damage.";
    public override string? AdditionalNotes { get; } = "They may not heal the superficial damage for the rest of the scene.";
    public override string Source { get; } = "Corebook, page 259";
}