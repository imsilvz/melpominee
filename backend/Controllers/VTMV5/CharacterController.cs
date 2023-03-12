using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Controllers;

[Authorize]
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

    [HttpGet("", Name = "Get Character List")]
    public VampireCharacterListResponse GetList()
    {
        // get email of logged in user
        string email = "";
        var identity = HttpContext.User.Identity;
        if (identity is null || 
            !identity.IsAuthenticated || 
            identity.Name is null)
        {
            return new VampireCharacterListResponse()
            {
                Error = "auth_error"
            };
        }
        email = identity.Name;

        // fetch character data
        var charList = VampireV5Character.GetCharactersByUser(email);
        var headerList = new List<VampireV5Header>();
        foreach(var character in charList)
        {
            headerList.Add(character.GetHeader());
        }

        // return!
        return new VampireCharacterListResponse()
        {
            Success = true,
            CharacterList = headerList,
        };
    }

    [HttpPut("{charId:int}", Name = "Update Character")]
    public VampireHeaderResponse Update(int charId, [FromBody] VampireCharacterUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireHeaderResponse
                {
                    Success = true,
                    Character = character.GetHeader()
                };
            }
        }
        return new VampireHeaderResponse
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
    public VampireAttributesResponse UpdateAttributes(int charId, [FromBody] VampireAttributesUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireAttributesResponse
                {
                    Success = true,
                    Attributes = character.Attributes
                };
            }
        }
        return new VampireAttributesResponse
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
    public VampireSkillsResponse UpdateSkills(int charId, [FromBody] VampireSkillsUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireSkillsResponse
                {
                    Success = true,
                    Skills = character.Skills
                };
            }
        }
        return new VampireSkillsResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpGet("disciplines/{charId:int}", Name = "Get Character Disciplines")]
    public VampireDisciplinesResponse GetDisciplines(int charId)
    {
        VampireV5Disciplines? disciplines;
        if(charId > 0)
        {
            disciplines = VampireV5Disciplines.Load(charId); 
            if(disciplines is not null)
            {
                return new VampireDisciplinesResponse
                {
                    Success = true,
                    Disciplines = disciplines,
                };
            }
        }
        return new VampireDisciplinesResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("disciplines/{charId:int}", Name = "Update Character Disciplines")]
    public VampireDisciplinesResponse UpdateDisciplines(int charId, [FromBody] VampireDisciplinesUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireDisciplinesResponse
                {
                    Success = true,
                    Disciplines = character.Disciplines
                };
            }
        }
        return new VampireDisciplinesResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpGet("powers/{charId:int}", Name = "Get Character Powers")]
    public VampirePowersResponse GetPowers(int charId)
    {
        VampireV5DisciplinePowers? powers;
        if(charId > 0)
        {
            powers = VampireV5DisciplinePowers.Load(charId); 
            if(powers is not null)
            {
                return new VampirePowersResponse
                {
                    Success = true,
                    Powers = powers.GetIdList(),
                };
            }
        }
        return new VampirePowersResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("powers/{charId:int}", Name = "Update Character Powers")]
    public VampirePowersResponse UpdatePowers(int charId, [FromBody] VampirePowersUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampirePowersResponse
                {
                    Success = true,
                    Powers = character.DisciplinePowers.GetIdList()
                };
            }
        }
        return new VampirePowersResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
}