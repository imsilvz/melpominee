namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class BaalsCaress : VampireDiscipline
{
    public override string Id { get; } = "baals_caress";
    public override string Name { get; } = "Baal’s Caress";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One or more Rouse Checks";
    public override string Duration { get; } = "One Scene";
    public override string DicePool { get; } = "Strength + Blood Sorcery";
    public override string OpposingPool { get; } = "Stamina + Occult/Fortitude";
    public override string Effect { get; } = "Change the user's own Vitae into an aggressive and lethal poison.";
    public override string? AdditionalNotes { get; } = "If a mortal takes one point of damage they die instantly.";
    public override string Source { get; } = "Corebook, page 251";
}