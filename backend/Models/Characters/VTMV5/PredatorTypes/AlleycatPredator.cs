namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class AlleycatPredator : VampirePredatorType
{
    public override string Id { get; } = "alleycat";
    public override string Name { get; } = "Alleycat";
    public override string Description { get; } = "Those who find violence to be the quickest way to get what they want might gravitate towards this hunting style. Alleycats are a vampire who feeds by brute force and outright attack and feeds from whomever they can when they can. Intimidation is a route easily taken to make their victims cower or even Dominating the victims to not report the attack or mask it as something else entirely.";
    public override string RollInfo { get; } = "Strength + Brawl is to take blood by force or threat. Wits + Streetwise can be used to find criminals as if a vigilante figure.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Intimidation (Stickups) or Brawl (Grappling)", 
        "Gain one dot of either Celerity or Potence", 
        "Lose one dot of Humanity", 
        "Gain three dots of Criminal Contacts" 
    };
}