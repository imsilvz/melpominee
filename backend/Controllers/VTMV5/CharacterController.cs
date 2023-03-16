using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Controllers;

[Authorize]
[ApiController]
[Route("vtmv5/[controller]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CharacterController : ControllerBase
{
    private readonly IHubContext<CharacterHub, ICharacterClient> _characterHub;
    private readonly ILogger<CharacterController> _logger;
    public CharacterController(ILogger<CharacterController> logger, IHubContext<CharacterHub, ICharacterClient> characterHub)
    {
        _logger = logger;
        _characterHub = characterHub;
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
    public async Task<VampireHeaderResponse> Update(int charId, [FromBody] VampireCharacterUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                await update.Apply(character);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnHeaderUpdate(charId, update);
                return new VampireHeaderResponse
                {
                    Success = (character is not null),
                    Character = character!.GetHeader()
                };
            }
        }
        return new VampireHeaderResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPost("", Name = "Create Character")]
    public VampireCharacterCreateResponse CreateCharacter()
    {
        // get email of logged in user
        string email = "";
        var identity = HttpContext.User.Identity;
        if (identity is null || 
            !identity.IsAuthenticated || 
            identity.Name is null)
        {
            return new VampireCharacterCreateResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        email = identity.Name;

        var character = new VampireV5Character()
        {
            Owner = email,
        };
        character.Save();

        return new VampireCharacterCreateResponse
        {
            Success = true,
            CharacterId = character.Id
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
    public async Task<VampireAttributesResponse> UpdateAttributes(int charId, [FromBody] VampireAttributesUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                await update.Apply(character);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnAttributeUpdate(charId, update);
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
    public async Task<VampireSkillsResponse> UpdateSkills(int charId, [FromBody] VampireSkillsUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                await update.Apply(character);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnSkillUpdate(charId, update);
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

    // secondary stats
    [HttpGet("stats/{charId:int}", Name = "Get Character Stats")]
    public VampireStatResponse GetSecondaryStats(int charId)
    {
        VampireV5SecondaryStats? stats;
        if(charId > 0)
        {
            stats = VampireV5SecondaryStats.Load(charId);
            if(stats is not null)
            {
                return new VampireStatResponse
                {
                    Success = true,
                    Stats = stats
                };
            }
        }
        return new VampireStatResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("stats/{charId:int}", Name = "Update Character Stats")]
    public VampireStatResponse UpdateSecondaryStats(int charId, [FromBody] VampireStatsUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireStatResponse
                {
                    Success = true,
                    Stats = character.SecondaryStats
                };
            }
        }
        return new VampireStatResponse
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

    [HttpGet("beliefs/{charId:int}", Name = "Get Character Beliefs")]
    public VampireBeliefsResponse GetBeliefs(int charId)
    {
        VampireV5Beliefs? beliefs;
        if(charId > 0)
        {
            beliefs = VampireV5Beliefs.Load(charId); 
            if(beliefs is not null)
            {
                return new VampireBeliefsResponse
                {
                    Success = true,
                    Beliefs = beliefs,
                };
            }
        }
        return new VampireBeliefsResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("beliefs/{charId:int}", Name = "Update Character Beliefs")]
    public VampireBeliefsResponse UpdateBeliefs(int charId, [FromBody] VampireBeliefsUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireBeliefsResponse
                {
                    Success = true,
                    Beliefs = character.Beliefs,
                };
            }
        }
        return new VampireBeliefsResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpGet("backgrounds/{charId:int}", Name = "Get Character Backgrounds")]
    public VampireBackgroundMeritFlawResponse GetBackgrounds(int charId)
    {
        VampireV5Backgrounds? backgrounds;
        if(charId > 0)
        {
            backgrounds = VampireV5Backgrounds.Load(charId);
            if(backgrounds is not null)
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Backgrounds = backgrounds,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("backgrounds/{charId:int}", Name = "Update Character Backgrounds")]
    public VampireBackgroundMeritFlawResponse UpdateBackgrounds(int charId, [FromBody] VampireBackgroundMeritFlawUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Backgrounds = character.Backgrounds,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpGet("merits/{charId:int}", Name = "Get Character Merits")]
    public VampireBackgroundMeritFlawResponse GetMerits(int charId)
    {
        VampireV5Merits? merits;
        if(charId > 0)
        {
            merits = VampireV5Merits.Load(charId);
            if(merits is not null)
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Merits = merits,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpPut("merits/{charId:int}", Name = "Update Character Merits")]
    public VampireBackgroundMeritFlawResponse UpdateMerits(int charId, [FromBody] VampireBackgroundMeritFlawUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Merits = character.Merits,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpGet("flaws/{charId:int}", Name = "Get Character Flaws")]
    public VampireBackgroundMeritFlawResponse GetFlaws(int charId)
    {
        VampireV5Flaws? flaws;
        if(charId > 0)
        {
            flaws = VampireV5Flaws.Load(charId);
            if(flaws is not null)
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Flaws = flaws,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("flaws/{charId:int}", Name = "Update Character Flaws")]
    public VampireBackgroundMeritFlawResponse UpdateFlaws(int charId, [FromBody] VampireBackgroundMeritFlawUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Flaws = character.Flaws,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
    
    [HttpGet("profile/{charId:int}", Name = "Get Character Profile")]
    public VampireProfileResponse GetProfile(int charId)
    {
        VampireV5Profile? profile;
        if(charId > 0)
        {
            profile = VampireV5Profile.Load(charId); 
            if(profile is not null)
            {
                return new VampireProfileResponse
                {
                    Success = true,
                    Profile = profile,
                };
            }
        }
        return new VampireProfileResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [HttpPut("profile/{charId:int}", Name = "Update Character Profile")]
    public VampireProfileResponse UpdateProfile(int charId, [FromBody] VampireProfileUpdate update)
    {
        VampireV5Character? character;
        if(charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId); 
            if(character is not null && character.Loaded)
            {
                update.Apply(character);
                return new VampireProfileResponse
                {
                    Success = true,
                    Profile = character.Profile,
                };
            }
        }
        return new VampireProfileResponse
        {
            Success = false,
            Error = "not_found"
        };
    }
}