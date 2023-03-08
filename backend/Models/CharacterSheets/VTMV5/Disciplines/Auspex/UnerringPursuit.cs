namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Auspex;

public class UnerringPursuit : VampirePower
{
    public override string Id { get; } = "unerring_pursuit";
    public override string Name { get; } = "Unerring Pursuit";
    public override string School { get; } = "Auspex";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = 1,
        School = "Dominate",
    };
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One night plus one for each success";
    public override string DicePool { get; } = "Resolve + Auspex";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Tracking a victim.";
    public override string? AdditionalNotes { get; } = "The victim can roll Wits + Awareness to catch a glimpse of the user once under its effects.";
    public override string Source { get; } = "Sabbat, page 46";
}