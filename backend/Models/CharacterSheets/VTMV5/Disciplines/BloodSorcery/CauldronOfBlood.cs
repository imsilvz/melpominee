namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.BloodSorcery;

public class CauldronOfBlood : VampireDiscipline
{
    public override string Id { get; } = "cauldron_of_blood";
    public override string Name { get; } = "Cauldron of Blood";
    public override string School { get; } = "Blood Sorcery";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check, Stains";
    public override string Duration { get; } = "One turn";
    public override string DicePool { get; } = "Resolve + Blood Sorcery";
    public override string OpposingPool { get; } = "Composure + Occult/Fortitude";
    public override string Effect { get; } = "Boil the victim's blood in their body.";
    public override string? AdditionalNotes { get; } = "If a mortal takes one point of damage they die screaming.";
    public override string Source { get; } = "Corebook, page 251";
}