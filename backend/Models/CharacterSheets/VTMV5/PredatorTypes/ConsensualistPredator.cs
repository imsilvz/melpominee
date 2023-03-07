namespace Melpominee.app.Models.CharacterSheets.VTMV5.PredatorTypes;

public class ConsensualistPredator : VampirePredatorType
{
    public override string Id { get; } = "consensualist";
    public override string Name { get; } = "Consensualist";
    public override string Description { get; } = "Consent is a dangerous thing to gather when they're a blood-sucking monster, but Consensualists make do. They never feed against the victim's free will, instead pretending to be a representative of a charity blood drive, someone with a blood kink within the kink community, or blatantly admitting to their victims what they are and getting their permission to feed. To the Camarilla, the last method is considered a masquerade breach but perhaps to a philosophical Anarch, it might be an acceptable risk to take.";
    public override string RollInfo { get; } = "Manipulation + Persuasion allows the kindred to take blood by consent, under the guide of medical work or mutual kink.";
    public override List<string> EffectList { get; } = new List<string> 
    { 
        "Gain one specialty in either Medicine (Phlebotomy) or Persuasion (Vessels)", 
        "Gain one dot of Auspex or Fortitude", 
        "Gain one dot of Humanity", 
        "Gain the Dark Secret Flaw: (•) Masquerade Breacher",
        "Gain the Feeding Flaw: (•) Prey Exclusion (Non-consenting)",
    };
}