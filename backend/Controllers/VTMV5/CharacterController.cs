using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.Characters.VTMV5;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using Melpominee.app.Services.Hubs;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
namespace Melpominee.app.Controllers;

[Authorize]
[ApiController]
[Route("vtmv5/[controller]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CharacterController : ControllerBase
{
    private readonly ILogger<CharacterController> _logger;
    private readonly IDistributedCache _cache;
    private readonly ConnectionService _connectionHelper;
    private readonly IHubContext<CharacterHub, ICharacterClient> _characterHub;
    private readonly CharacterService _characterService;
    private readonly UserManager _userManager;
    public CharacterController(
        ILogger<CharacterController> logger,
        IDistributedCache cache,
        ConnectionService connectionHelper,
        IHubContext<CharacterHub, ICharacterClient> characterHub,
        CharacterService characterService,
        UserManager userManager)
    {
        _logger = logger;
        _cache = cache;
        _characterHub = characterHub;
        _connectionHelper = connectionHelper;
        _characterService = characterService;
        _userManager = userManager;
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("{charId:int}", Name = "Get Character")]
    public async Task<VampireCharacterResponse> Get(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireCharacterResponse()
            {
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                // hide owner email
                if (user.Role != "admin")
                {
                    if (user.Email != character.Owner)
                    {
                        character.Owner = null;
                    }
                }
                else
                {
                    var characterOwner = await _userManager.GetUser(character.Owner)!;
                    if (!string.IsNullOrEmpty(characterOwner?.DiscordName))
                    {
                        character.Owner = user.DiscordName;
                    }
                }

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
    public async Task<VampireCharacterListResponse> GetList([FromQuery] bool? adminView = false)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireCharacterListResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        // fetch character data
        List<VampireV5Character>? charList;
        var headerList = new List<VampireV5Header>();
        if (adminView == true)
        {
            charList = await VampireV5Character.GetCharacters();
        }
        else
        {
            charList = await VampireV5Character.GetCharactersByUser(identity.Name);
        }
        
        foreach (var character in charList)
        {
            // hide owner email
            if (user.Role != "admin")
            {
                if (user.Email != character.Owner)
                {
                    character.Owner = null;
                }
            }
            else
            {
                var characterOwner = await _userManager.GetUser(character.Owner)!;
                if (!string.IsNullOrEmpty(characterOwner?.DiscordName))
                {
                    character.Owner = characterOwner.DiscordName;
                }
            }
            headerList.Add(character.GetHeader());
        }

        // return!
        return new VampireCharacterListResponse()
        {
            Success = true,
            CharacterList = headerList,
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("{charId:int}", Name = "Update Character")]
    public async Task<VampireHeaderResponse> Update(int charId, [FromBody] CharacterUpdateWrapper<VampireCharacterUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireHeaderResponse()
            {
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireHeaderResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnHeaderUpdate(charId, update.UpdateId, startTime, update.UpdateData);
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
    public async Task<VampireCharacterCreateResponse> CreateCharacter()
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireCharacterCreateResponse()
            {
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        var character = new VampireV5Character()
        {
            Owner = user.Email,
        };
        character.Save();

        return new VampireCharacterCreateResponse
        {
            Success = true,
            CharacterId = character.Id
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("attributes/{charId:int}", Name = "Get Character Attributes")]
    public async Task<VampireAttributesResponse> GetAttributes(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireAttributesResponse()
            {
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Attributes? attributes;
        if (charId > 0)
        {
            attributes = await _characterService
                .GetCharacterProperty<VampireV5Attributes>(charId);
            if (attributes is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("attributes/{charId:int}", Name = "Update Character Attributes")]
    public async Task<VampireAttributesResponse> UpdateAttributes(int charId, [FromBody] CharacterUpdateWrapper<VampireAttributeUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireAttributesResponse()
            {
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireAttributesResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnAttributeUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireAttributesResponse
                {
                    Success = true,
                    Attributes = character!.Attributes
                };
            }
        }
        return new VampireAttributesResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("skills/{charId:int}", Name = "Get Character Skills")]
    public async Task<VampireSkillsResponse> GetSkills(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireSkillsResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Skills? skills;
        if (charId > 0)
        {
            skills = await _characterService
                .GetCharacterProperty<VampireV5Skills>(charId);
            if (skills is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("skills/{charId:int}", Name = "Update Character Skills")]
    public async Task<VampireSkillsResponse> UpdateSkills(int charId, [FromBody] CharacterUpdateWrapper<VampireSkillUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireSkillsResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireSkillsResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnSkillUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireSkillsResponse
                {
                    Success = true,
                    Skills = character!.Skills
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
    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("stats/{charId:int}", Name = "Get Character Stats")]
    public async Task<VampireStatResponse> GetSecondaryStats(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireStatResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5SecondaryStats? stats;
        if (charId > 0)
        {
            stats = await _characterService
                .GetCharacterProperty<VampireV5SecondaryStats>(charId);
            if (stats is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("stats/{charId:int}", Name = "Update Character Stats")]
    public async Task<VampireStatResponse> UpdateSecondaryStats(int charId, [FromBody] CharacterUpdateWrapper<VampireStatsUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireStatResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireStatResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnSecondaryUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireStatResponse
                {
                    Success = true,
                    Stats = character!.SecondaryStats
                };
            }
        }
        return new VampireStatResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("disciplines/{charId:int}", Name = "Get Character Disciplines")]
    public async Task<VampireDisciplinesResponse> GetDisciplines(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireDisciplinesResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Disciplines? disciplines;
        if (charId > 0)
        {
            disciplines = await _characterService
                .GetCharacterProperty<VampireV5Disciplines>(charId);
            if (disciplines is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("disciplines/{charId:int}", Name = "Update Character Disciplines")]
    public async Task<VampireDisciplinesResponse> UpdateDisciplines(int charId, [FromBody] CharacterUpdateWrapper<VampireDisciplineUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireDisciplinesResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireDisciplinesResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnDisciplineUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireDisciplinesResponse
                {
                    Success = true,
                    Disciplines = character!.Disciplines
                };
            }
        }
        return new VampireDisciplinesResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("powers/{charId:int}", Name = "Get Character Powers")]
    public async Task<VampirePowersResponse> GetPowers(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampirePowersResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5DisciplinePowers? powers;
        if (charId > 0)
        {
            powers = await _characterService
                .GetCharacterProperty<VampireV5DisciplinePowers>(charId);
            if (powers is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("powers/{charId:int}", Name = "Update Character Powers")]
    public async Task<VampirePowersResponse> UpdatePowers(int charId, [FromBody] CharacterUpdateWrapper<VampirePowersUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampirePowersResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampirePowersResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnPowersUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampirePowersResponse
                {
                    Success = true,
                    Powers = character!.DisciplinePowers.GetIdList()
                };
            }
        }
        return new VampirePowersResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("beliefs/{charId:int}", Name = "Get Character Beliefs")]
    public async Task<VampireBeliefsResponse> GetBeliefs(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBeliefsResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Beliefs? beliefs;
        if (charId > 0)
        {
            beliefs = await _characterService
                .GetCharacterProperty<VampireV5Beliefs>(charId);
            if (beliefs is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("beliefs/{charId:int}", Name = "Update Character Beliefs")]
    public async Task<VampireBeliefsResponse> UpdateBeliefs(int charId, [FromBody] CharacterUpdateWrapper<VampireBeliefsUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBeliefsResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireBeliefsResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnBeliefsupdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireBeliefsResponse
                {
                    Success = true,
                    Beliefs = character!.Beliefs,
                };
            }
        }
        return new VampireBeliefsResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("backgrounds/{charId:int}", Name = "Get Character Backgrounds")]
    public async Task<VampireBackgroundMeritFlawResponse> GetBackgrounds(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Backgrounds? backgrounds;
        if (charId > 0)
        {
            backgrounds = await _characterService
                .GetCharacterProperty<VampireV5Backgrounds>(charId);
            if (backgrounds is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("backgrounds/{charId:int}", Name = "Update Character Backgrounds")]
    public async Task<VampireBackgroundMeritFlawResponse> UpdateBackgrounds(int charId, [FromBody] CharacterUpdateWrapper<VampireBackgroundMeritFlawUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireBackgroundMeritFlawResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnBackgroundMeritFlawUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Backgrounds = character!.Backgrounds,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("merits/{charId:int}", Name = "Get Character Merits")]
    public async Task<VampireBackgroundMeritFlawResponse> GetMerits(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Merits? merits;
        if (charId > 0)
        {
            merits = await _characterService
                .GetCharacterProperty<VampireV5Merits>(charId);
            if (merits is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("merits/{charId:int}", Name = "Update Character Merits")]
    public async Task<VampireBackgroundMeritFlawResponse> UpdateMerits(int charId, [FromBody] CharacterUpdateWrapper<VampireBackgroundMeritFlawUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireBackgroundMeritFlawResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnBackgroundMeritFlawUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Merits = character!.Merits,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("flaws/{charId:int}", Name = "Get Character Flaws")]
    public async Task<VampireBackgroundMeritFlawResponse> GetFlaws(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Flaws? flaws;
        if (charId > 0)
        {
            flaws = await _characterService
                .GetCharacterProperty<VampireV5Flaws>(charId);
            if (flaws is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("flaws/{charId:int}", Name = "Update Character Flaws")]
    public async Task<VampireBackgroundMeritFlawResponse> UpdateFlaws(int charId, [FromBody] CharacterUpdateWrapper<VampireBackgroundMeritFlawUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireBackgroundMeritFlawResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireBackgroundMeritFlawResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnBackgroundMeritFlawUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = true,
                    Flaws = character!.Flaws,
                };
            }
        }
        return new VampireBackgroundMeritFlawResponse
        {
            Success = false,
            Error = "not_found"
        };
    }

    [Authorize(Policy = "CanViewCharacter")]
    [HttpGet("profile/{charId:int}", Name = "Get Character Profile")]
    public async Task<VampireProfileResponse> GetProfile(int charId)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireProfileResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        VampireV5Profile? profile;
        if (charId > 0)
        {
            profile = await _characterService
                .GetCharacterProperty<VampireV5Profile>(charId);
            if (profile is not null)
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

    [Authorize(Policy = "CanViewCharacter")]
    [HttpPut("profile/{charId:int}", Name = "Update Character Profile")]
    public async Task<VampireProfileResponse> UpdateProfile(int charId, [FromBody] CharacterUpdateWrapper<VampireProfileUpdate> update)
    {
        // get email of logged in user
        User user;
        var identity = HttpContext.User.Identity;
        if (identity is null ||
            !identity.IsAuthenticated ||
            identity.Name is null)
        {
            return new VampireProfileResponse()
            {
                Success = false,
                Error = "auth_error"
            };
        }
        user = (await _userManager.GetUser(identity.Name))!;

        if (update.UpdateData is null)
        {
            return new VampireProfileResponse
            {
                Success = false,
                Error = "missing_payload"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        VampireV5Character? character;
        if (charId > 0)
        {
            character = await _characterService
                .GetCharacterProperty<VampireV5Character>(charId);
            if (character is not null)
            {
                await update.UpdateData.Apply(character!, _cache);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnProfileUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireProfileResponse
                {
                    Success = true,
                    Profile = character!.Profile,
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