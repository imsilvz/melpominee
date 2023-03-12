using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class CharacterSheetResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Character? Character { get; set; }
}