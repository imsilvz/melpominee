namespace Melpominee.app.Models.CharacterSheets.VTMV5.PredatorTypes;

public class GraverobberPredator : VampirePredatorType
{
    public override string Id { get; } = "graverobber";
    public override string Name { get; } = "Graverobber";
    public override string Description { get; } = "Similar to Baggers these kindred understand there's no good in wasting good blood, even if others cannot consume it. Often they find themselves digging up corpses or working or mortuaries to obtain their bodies, yet regardless of what the name suggests, they prefer feeding from mourners at a gravesite or a hospital. This Predator Type often requires a haven or other connections to a church, hospital, or morgue as a way to obtain the bodies.";
    public override string RollInfo { get; } = "Resolve + Medicine for sifting through the dead for a body with blood. Manipulation + Insight for moving among miserable mortals.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one in specialty either Occult (Grave Rituals) or Medicine (Cadavers)", 
        "Gain one dot of Fortitude or Oblivion", 
        "Gain the Feeding Merit (•••) Iron Gullet",
        "Gain the Haven Advantage (•)",
        "Gain the Herd Flaw: (••) Obvious Predator"
    };
}