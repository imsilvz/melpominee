using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Controllers;

[ApiController]
[Route("vtmv5/[controller]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CharacterController : ControllerBase
{
    private readonly ILogger<CharacterController> _logger;
    public CharacterController(ILogger<CharacterController> logger)
    {
        _logger = logger;
    }

    [ActionName("")]
    [Route("{charId}")]
    [HttpGet(Name = "Get Character")]
    public CharacterSheetResponse Get(int charId)
    {
        VampireV5Sheet? sheet;
        if(charId > 0)
        {
            sheet = VampireV5Sheet.GetCharacter(charId); 
            if(sheet is not null && sheet.Loaded)
            {
                return new CharacterSheetResponse
                {
                    Success = true,
                    Character = sheet
                };
            }
        }
        return new CharacterSheetResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
}