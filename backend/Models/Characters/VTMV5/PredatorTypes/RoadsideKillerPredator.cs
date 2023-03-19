namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class RoadsideKillerPredator : VampirePredatorType
{
    public override string Id { get; } = "roadside_killer";
    public override string Name { get; } = "Roadside Killer";
    public override string Description { get; } = "These Kindred never stay in one spot for too long and are always on the move, hunting those who won't be missed if they disappear alongside the road. Roadside Killers know the risk is just as worth as the reward. Perhaps this Kindred was once a truck driver themselves or maybe they met their fate alongside the road as well.";
    public override string RollInfo { get; } = "Dexterity/Charisma + Drive to feed by picking up down and outs with no other options.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one in specialty either Survival (the road) or Investigation (vampire cant)", 
        "Gain one dot of Fortitude or Protean", 
        "Gain two additional dots of migrating Herd",
        "Gain the Feeding Flaw: Prey Exclusion (locals)"
    };
}