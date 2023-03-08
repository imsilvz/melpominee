namespace Melpominee.app.Models.CharacterSheets.VTMV5.Disciplines.Obfuscate;

public class MaskOfAThousandFaces : VampirePower
{
    public override string Id { get; } = "mask_of_a_thousand_faces";
    public override string Name { get; } = "Mask of a Thousand Faces";
    public override string School { get; } = "Obfuscate";
    public override int Level { get; } = 3;
    public override string Cost { get; } = "One Rouse Check";
    public override string Duration { get; } = "One scene";
    public override string DicePool { get; } = "N/A";
    public override string OpposingPool { get; } = "N/A";
    public override string Effect { get; } = "Make themselves appear as a mundane face rather than disappear allowing interaction and communication.";
    public override string? AdditionalNotes { get; } = "This power allows them to interact and speak to others around them.";
    public override string Source { get; } = "Corebook, page 262";
}