namespace Melpominee.app.Models.Characters.VTMV5.PredatorTypes;

public class DefaultPredator : VampirePredatorType
{
    public override string Id { get; } = "";
    public override string Name { get; } = "N/A";
    public override string Description { get; } = "";
    public override string RollInfo { get; } = "";
    public override List<string> EffectList { get; } = new List<string>();
}