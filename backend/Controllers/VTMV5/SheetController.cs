using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
    public VampireV5Sheet? GetCharacter(int charId)
    {
        VampireV5Sheet sheet;
        if(charId > 0)
        {
            sheet = new VampireV5Sheet(charId); 
            if(sheet.Loaded)
            {
                return sheet;
            }
        }
        return null;
    }
}