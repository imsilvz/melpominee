namespace Melpominee.app.Models.CharacterSheets.VTMV5.PredatorTypes;

public class OsirisPredator : VampirePredatorType
{
    public override string Id { get; } = "osiris";
    public override string Name { get; } = "Osiris";
    public override string Description { get; } = "More than not, Osiris are celebrities within mortal society. Musicians, writers, priests, and even cult leaders may find an easy time finding their blood by utilizing those already around them. They tend to feed from their fans or worshippers which means they have easy access to blood, but followers tend to attract their own problems with the local authority or worse.";
    public override string RollInfo { get; } = "Manipulation + Subterfuge or Intimidation + Fame are both used to feed from the adoring fans.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Occult (specific tradition) or Performance (specific entertainment field)", 
        "Gain one dot of Blood Sorcery (Tremere or Banu Haqim only) or Presence", 
        "Spend three dots between the Fame and Herd Backgrounds", 
        "Spend two dots between Enemies and Mythic Flaws",
    };
}