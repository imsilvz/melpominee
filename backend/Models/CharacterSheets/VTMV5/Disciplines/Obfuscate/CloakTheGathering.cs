namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class CloakTheGathering : VampireDiscipline
{
    public override string Id { get; } = "cloak_the_gathering";
    public override string Name { get; } = "Cloak the Gathering";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One Rouse Check in additional to the cost of the power extended";
    public override string Duration { get; } = "As per power extended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Shelter companions under Obfuscate.";
    public override string? AdditionalNotes { get; } = "This power extends to a number of people equal to the user's Wits, plus additional Rouse Checks.";
    public override string Source { get; } = "Corebook, page 263";
}