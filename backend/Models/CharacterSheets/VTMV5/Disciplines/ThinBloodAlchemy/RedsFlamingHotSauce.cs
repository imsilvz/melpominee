namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.ThinBloodAlchemy;

public class RedsFlamingHotSauce : VampirePower
{
    public override string Id { get; } = "reds_flaming_hot_sauce";
    public override string Name { get; } = "Red's Flaming Hot Sauce";
    public override string School { get; } = "Thin-Blood Alchemy";
    public override int Level { get; } = 2;
    public override VampirePowerAmalgam? Amalgam { get; } = new VampirePowerAmalgam
    {
        Level = null,
        School = "Choleric",
    };
    public override string Cost { get; } = "N/A";
    public override string Duration { get; } = "Until used, the fire lasts one scene or longer based on the surroundings";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Create a supernatural molotov cocktail";
    public override string? AdditionalNotes { get; } = "Fire spreading from it can be extinguished, but the supernatural source cannot.";
    public override string Source { get; } = "Winter's Teeth, Page 10";
}