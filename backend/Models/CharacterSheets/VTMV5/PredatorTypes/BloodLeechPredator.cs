namespace Melpominee.app.Models.CharacterSheets.VTMV5.PredatorTypes;

public class BloodLeechPredator : VampirePredatorType
{
    public override string Id { get; } = "blood_leech";
    public override string Name { get; } = "Blood Leech";
    public override string Description { get; } = "Some Kindred might see feeding from mortals as inherently wrong or disgusting regardless of others' rationale. Blood Leech is a feeding style that is not looked upon kindly by many vampires making it risky unless the Kindred has a position of power and can keep their little secret secure. Regardless, with their rejection of mortal blood, they instead feed upon the vitae of other vampires through hunting those weaker than them, coercion, or taking Blood as payment.";
    public override string RollInfo { get; } = "This Predator Type is suggested to not be abstracted down to a dice pool";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Brawl (Kindred) or Stealth (Against Kindred)", 
        "Gain one dot of Celerity or Protean", 
        "Lose one dot of Humanity", 
        "Increase blood potency by one",
        "Gain the Dark Secret Flaw: Diablerist (••), or the Shunned Flaw (••)",
        "Gain the Feeding Flaw: (••) Prey Exclusion (Mortals)"
    };
}