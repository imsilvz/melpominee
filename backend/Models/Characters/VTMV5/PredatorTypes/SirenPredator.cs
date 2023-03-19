namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class SirenPredator : VampirePredatorType
{
    public override string Id { get; } = "siren";
    public override string Name { get; } = "Siren";
    public override string Description { get; } = "Everyone knows that sex sells and the Siren uses this to their advantage. Almost exclusively feeding while feigning sex or sexual interest, they utilize Disciplines and seduction to lure away a possible meal. Moving through clubs and one-night stands are skills they've mastered and regardless of how sexy they feel, deep in their darkest moments, they realize at best they are problematic and at worst a serial sexual assaulter. In life, they might have been a scriptwriter, a small time actor who never reached the big screen, a well-known kinkster or even a virgin looking to make up for the lost time.";
    public override string RollInfo { get; } = "Charisma + Subterfuge is how sirens feed under the guise of sexual acts.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Persuasion (Seduction) or Subterfuge (Seduction)", 
        "Gain one dot of Fortitude or Presence", 
        "Gain the Looks Merit: (••) Beautiful",
        "Gain the Enemy Flaw (•) A spurned lover or jealous partner"
    };
}