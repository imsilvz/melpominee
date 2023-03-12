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
    public VampireCharacterResponse Get(int charId)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                return new VampireCharacterResponse
                {
                    Success = true,
                    Character = character
                };
            }
        }
        return new VampireCharacterResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("{charId:int}", Name = "Update Character")]
    public VampireCharacterResponse Update(int charId, [FromBody] VampireCharacterUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireCharacterResponse
                {
                    Success = true,
                    Character = character
                };
            }
        }
        return new VampireCharacterResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpGet("attributes/{charId:int}", Name = "Get Character Attributes")]
    public VampireAttributesResponse GetAttributes(int charId)
    {
        VampireV5Attributes? attributes;
        if(charId > 0)
        {
            attributes = VampireV5Attributes.Load(charId); 
            if(attributes is not null)
            {
                return new VampireAttributesResponse
                {
                    Success = true,
                    Attributes = attributes
                };
            }
        }
        return new VampireAttributesResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("attributes/{charId:int}", Name = "Update Character Attributes")]
    public VampireCharacterResponse UpdateAttributes(int charId, [FromBody] VampireAttributesUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireCharacterResponse
                {
                    Success = true,
                    Character = character
                };
            }
        }
        return new VampireCharacterResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpGet("skills/{charId:int}", Name = "Get Character Skills")]
    public VampireSkillsResponse GetSkills(int charId)
    {
        VampireV5Skills? skills;
        if(charId > 0)
        {
            skills = VampireV5Skills.Load(charId); 
            if(skills is not null)
            {
                return new VampireSkillsResponse
                {
                    Success = true,
                    Skills = skills,
                };
            }
        }
        return new VampireSkillsResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("skills/{charId:int}", Name = "Update Character Skills")]
    public VampireCharacterResponse UpdateSkills(int charId, [FromBody] VampireSkillsUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireCharacterResponse
                {
                    Success = true,
                    Character = character
                };
            }
        }
        return new VampireCharacterResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
}