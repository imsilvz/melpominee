namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class BaggerPredator : VampirePredatorType
{
    public override string Id { get; } = "bagger";
    public override string Name { get; } = "Bagger";
    public override string Description { get; } = "Sometimes the best blood doesn't come from a live body. Baggers are kindred who take an approach most are unable to with their ability to consume preserved, defractionated or rancid blood through (•••) Iron Gullet, allowing them to feed from unusual sources such as blood bags or corpses. Perhaps they work in a hospital or blood bank or they might even have enough knowledge about the black market to obtain their blood. Ventrue are unable to pick this Predator type.";
    public override string RollInfo { get; } = "Intelligence + Streetwise can be used to find, gain access and purchase the goods.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Larceny (Lock Picking) or Streetwise (Black Market)", 
        "Gain one dot of Blood Sorcery (Tremere and Banu Haqim only), Oblivion (Hecata and Lasombra only), or Obfuscate", 
        "Gain the Feeding Merit (•••) Iron Gullet", 
        "Gain an Enemy Flaw (••) of someone who believes this vampire owes them something or there's another reason to hunt them down" 
    };
}