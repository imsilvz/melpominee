namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class ExtortionistPredator : VampirePredatorType
{
    public override string Id { get; } = "extortionist";
    public override string Name { get; } = "Extortionist";
    public override string Description { get; } = "On the surface, Extortionists acquire their blood in exchange for services such as protection, security, or surveillance. Though, for as many times as the service might be genuine, there are many more times when the service has been offered from fabricated information to make the deal feel that much sweeter.";
    public override string RollInfo { get; } = "Strength/Manipulation + Intimidation to feed through coercion.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Intimidation(Coercion) or Larceny(Security)", 
        "Gain one dot of Dominate or Potence", 
        "Spend three dots between the Contacts and Resources Backgrounds",
        "Gain the Enemy Flaw (••) The police or a victim who escaped the character's extortion and wants revenge"
    };
}