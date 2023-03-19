namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class SandmanPredator : VampirePredatorType
{
    public override string Id { get; } = "sandman";
    public override string Name { get; } = "Sandman";
    public override string Description { get; } = "If they never wake during the feed it never happened, right? Sandman prefers to hunt on sleeping mortals than anyone else by using stealth or Disciplines to feed from their victims they are rarely caught in the act, though when they are, problems are sure to occur. Maybe they were anti-social in life or perhaps they find the route of seduction or violence too much for them and find comfort in the silence of this feeding style.";
    public override string RollInfo { get; } = "Dexterity + Stealth is for casing a location, breaking in and feeding without leaving a trace.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Medicine (Anesthetics) or Stealth (Break-in)", 
        "Gain one dot of Auspex or Obfuscate", 
        "Gain one dot of Resources"
    };
}