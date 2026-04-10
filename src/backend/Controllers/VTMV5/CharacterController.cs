using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.Characters.VTMV5;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
namespace Melpominee.app.Controllers;

[Authorize]
[ApiController]
[Route("vtmv5/[controller]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CharacterController : ControllerBase
{
    private readonly ILogger<CharacterController> _logger;
    private readonly IHubContext<CharacterHub, ICharacterClient> _characterHub;
    private readonly CharacterService _characterService;
    private readonly CharacterCommandDispatcher _dispatcher;
    private readonly UserManager _userManager;
    public CharacterController(
        ILogger<CharacterController> logger,
        IHubContext<CharacterHub, ICharacterClient> characterHub,
        CharacterService characterService,
        CharacterCommandDispatcher dispatcher,
        UserManager userManager)
    {
        _logger = logger;
        _characterHub = characterHub;
        _characterService = characterService;
        _dispatcher = dispatcher;
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
                if (user.Email != character.Owner)
                {
                    character.Owner = null;
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
        if (adminView == true) {
          if (user.Role != "admin") {
            return new VampireCharacterListResponse {
              Success = false,
              Error = "forbidden"
            };
          }
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
                else
                {
                    // hide username if not signing in through discord
                    if (string.IsNullOrEmpty(characterOwner?.Password))
                    {
                        character.Owner = null;
                    }
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
    public async Task<CharacterCommandResponse> UpdateCommands(
        int charId,
        [FromBody] CharacterCommandRequest request) {
        if (request.Commands is null || request.Commands.Count == 0) {
            return new CharacterCommandResponse {
                Success = false,
                Error = "missing_commands"
            };
        }

        if (request.Commands.Count > 20) {
            return new CharacterCommandResponse {
                Success = false,
                Error = "too_many_commands"
            };
        }

        if (charId <= 0) {
            return new CharacterCommandResponse {
                Success = false,
                Error = "invalid_character_id"
            };
        }

        var character = await _characterService
            .GetCharacterProperty<VampireV5Character>(charId);
        if (character is null) {
            return new CharacterCommandResponse {
                Success = false,
                Error = "not_found"
            };
        }

        DateTime startTime = DateTime.UtcNow;
        List<CharacterCommand> applied;
        try {
            applied = await _dispatcher
                .ApplyCommands(character, request.Commands);
        } catch (ArgumentException ex) {
            _logger.LogWarning(ex,
                "Command dispatch failed for character {CharId}",
                charId);
            return new CharacterCommandResponse {
                Success = false,
                Error = "invalid_command"
            };
        }

        // Broadcast new unified event
        _ = _characterHub.Clients
            .Group($"character_{charId}")
            .OnCharacterUpdate(
                charId, request.UpdateId, startTime, applied)
            .ContinueWith(
                t => _logger.LogError(t.Exception,
                    "SignalR broadcast failed for character {CharId}",
                    charId),
                TaskContinuationOptions.OnlyOnFaulted);

        return new CharacterCommandResponse { Success = true };
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
}
