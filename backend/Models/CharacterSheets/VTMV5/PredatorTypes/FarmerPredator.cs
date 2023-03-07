namespace Melpominee.app.Models.CharacterSheets.VTMV5.PredatorTypes;

public class FarmerPredator : VampirePredatorType
{
    public override string Id { get; } = "farmer";
    public override string Name { get; } = "Farmer";
    public override string Description { get; } = "Perhaps this vampire was once someone who worked as an activist or an aid worker, regardless of their reasoning the Farmer only feed from animals as their primary source of blood. The beast may gnaw at them with its throes of hunger, but they've successfully managed to avoid killing mortals except on the occasional bad night. Ventrue may not pick this Predator type and it cannot be taken on characters with Blood Potency 3 or higher.";
    public override string RollInfo { get; } = "Composure + Animal Ken is the roll to find and catch the chosen animal.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Animal Ken (specific animal) or Survival (Hunting)", 
        "Gain one dot of Animalism or Protean", 
        "Gain one dot of Humanity", 
        "Gain the Feeding Flaw: (••) Farmer",
    };
}