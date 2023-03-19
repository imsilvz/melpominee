namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class CleaverPredator : VampirePredatorType
{
    public override string Id { get; } = "cleaver";
    public override string Name { get; } = "Cleaver";
    public override string Description { get; } = "The sweetest blood might be from those closest to them, the Cleaver takes advantage of that idea while taking blood from either their own close family and friends or even those close to someone else. Covertly stealing the blood from their victims while still maintaining ties to them. Cleavers will go to extreme lengths to keep their condition a secret from their victims but some may instead take a less than pleasant route. The Camarilla forbids the practice of taking a human family in this fashion, as it's a breach waiting to happen.";
    public override string RollInfo { get; } = "Manipulation + Subterfuge is used to condition the victims, socializing with them and feeding from them without the cover being blown.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Persuasion (Gaslighting) or Subterfuge (Coverups)", 
        "Gain one dot of Dominate or Animalism", 
        "Gain the Dark Secret Flaw (•) Cleaver", 
        "Gain the Herd Advantage (••)",
    };
}