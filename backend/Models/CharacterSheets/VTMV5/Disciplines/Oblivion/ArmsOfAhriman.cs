namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Oblivion;

public class ArmsOfAhriman : VampirePower
{
    public override string Id { get; } = "arms_of_ahriman";
    public override string Name { get; } = "Arms of Ahriman";
    public override string School { get; } = "Oblivion";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 2,
        School = "Potence",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene or until ended or destroyed";
    public override string DicePool { get; } = "Wits + Oblivion";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Conjures shadow appendages that the user can control but the user is unable to do anything else.";
    public override string? AdditionalNotes { get; } = "The shadow can be extended up to twice the user's Oblivion rating in yards/meters.";
    public override string Source { get; } = "Chicago by Night, page 294";
}