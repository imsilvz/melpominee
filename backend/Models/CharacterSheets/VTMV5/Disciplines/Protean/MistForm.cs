namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Protean;

public class MistForm : VampireDiscipline
{
    public override string Id { get; } = "mist_form";
    public override string Name { get; } = "Mist Form";
    public override string School { get; } = "Protean";
    public override int Level { get; } = 5;
    public override string Cost { get; } = "One to three Rouse Checks";
    public override string Duration { get; } = "One scene unless voluntarily ended";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Turn into a cloud of mist.";
    public override string? AdditionalNotes { get; } = "This power takes three turns to use and may be sped up with additional Rouse Checks on a one-for-one trade.";
    public override string Source { get; } = "Corebook, page 271";
}