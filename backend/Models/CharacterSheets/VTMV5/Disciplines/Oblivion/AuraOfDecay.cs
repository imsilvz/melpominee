namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class AuraOfDecay : VampirePower
{
    public override string Id { get; } = "aura_of_decay";
    public override string Name { get; } = "Aura of Decay";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "Stamina + Oblivion";
    public override string OpposingPool { get; } = "Stamina + Medicine/Fortitude";
    public override string Effect { get; } = "Harnessing their connection to Oblivion can make plants wilt, animals and humans sick, and food spoil.";
    public override string? AdditionalNotes { get; } = "Repeated use of this power within the same scene does not affect Mortals already damaged by it.";
    public override string Source { get; } = "Cults of the Blood Gods, page 205";
}