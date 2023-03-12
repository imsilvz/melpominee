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

    [HttpGet("{charId:int}", Name = "Get Character")]
    public CharacterSheetResponse Get(int charId)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                return new CharacterSheetResponse
                {
                    Success = true,
                    Character = character
                };
            }
        }
        return new CharacterSheetResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("{charId:int}", Name = "Update Character")]
    public CharacterSheetResponse Update(int charId, [FromBody] VampireCharacterUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new CharacterSheetResponse
                {
                    Success = true,
                    Character = character
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