namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class SceneQueenPredator : VampirePredatorType
{
    public override string Id { get; } = "scene_queen";
    public override string Name { get; } = "Scene Queen";
    public override string Description { get; } = "Similar to Osiris these Kindred find comfort in a particular subculture rather than a wider audience. Hunting in or around a subculture they likely belonged to in their previous life, their victims adore them for their status, and those who have an inkling of what they are disbelieved. The scene itself could be anything, from street culture to high fashion, and the unifying trait is the use of those around them.";
    public override string RollInfo { get; } = "Manipulation + Persuasion aids in feeding from those within the Kindred's subgroup, through conditioning and isolation to gain blood or gaslighting or forced silence.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Etiquette (specific scene), Leadership (specific scene), or Streetwise (specific scene)", 
        "Gain one dot of Dominate or Potence", 
        "Gain the Fame Advantage (•)",
        "Gain the Contact Advantage (•)",
        "Gain either the Influence Flaw: (•) Disliked (outside their subculture) or the Feeding Flaw: (•) Prey Exclusion (a different subculture than theirs)"
    };
}