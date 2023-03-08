namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class NecroticPlague : VampirePower
{
    public override string Id { get; } = "necrotic_plague";
    public override string Name { get; } = "Necrotic Plague";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 4;
    public override string Cost { get; } = "One Rouse Check, Two Stains";
    public override string Duration { get; } = "One turn to activate, variable length of the condition";
    public override string DicePool { get; } = "Intelligence + Oblivion";
    public override string OpposingPool { get; } = "Stamina + Medicine/Fortitude";
    public override string Effect { get; } = "Manifest illness in victims.";
    public override string? AdditionalNotes { get; } = "This illness cannot be treated in a medical setting as it's supernaturally inflicted, instead only healed by drinking vitae.";
    public override string Source { get; } = "Cults of the Blood Gods, page 206";
}