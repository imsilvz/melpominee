using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
using Melpominee.app.Services.Hubs;
using Melpominee.app.Services.Auth;
namespace Melpominee.app.Controllers;

[Authorize]
[ApiController]
[Route("vtmv5/[controller]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class CharacterController : ControllerBase
{
    private readonly ConnectionService _connectionHelper;
    private readonly IHubContext<CharacterHub, ICharacterClient> _characterHub;
    private readonly ILogger<CharacterController> _logger;
    public CharacterController(
        ILogger<CharacterController> logger,
        IHubContext<CharacterHub, ICharacterClient> characterHub,
        ConnectionService connectionHelper)
    {
        _logger = logger;
        _characterHub = characterHub;
        _connectionHelper = connectionHelper;
    }

    [HttpGet("{charId:int}", Name = "Get Character")]
    public VampireCharacterResponse Get(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Character? character;
        if (charId > 0)
        {
            character = VampireV5Character.GetCharacter(charId);
            // check if allowed!
            if (user.CanViewCharacter(character))
            {
                return new VampireCharacterResponse
                {
                    Success = true,
                    Character = character
                };
            }
            return new VampireCharacterResponse
            {
                Success = false,
                Error = "access_error"
            };
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        // fetch character data
        var charList = VampireV5Character.GetCharactersByUser(identity.Name);
        var headerList = new List<VampireV5Header>();
        foreach (var character in charList)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            // check if allowed!
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnHeaderUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireHeaderResponse
                {
                    Success = (character is not null),
                    Character = character!.GetHeader()
                };
            }
            return new VampireHeaderResponse
            {
                Success = false,
                Error = "access_error"
            };
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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

    [HttpGet("attributes/{charId:int}", Name = "Get Character Attributes")]
    public VampireAttributesResponse GetAttributes(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Attributes? attributes;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                attributes = VampireV5Attributes.Load(charId);
                if (attributes is not null)
                {
                    return new VampireAttributesResponse
                    {
                        Success = true,
                        Attributes = attributes
                    };
                }
            }
            else
            {
                return new VampireAttributesResponse
                {
                    Success = false,
                    Error = "access_error",
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
    public async Task<VampireAttributesResponse> UpdateAttributes(int charId, [FromBody] CharacterUpdateWrapper<VampireAttributesUpdate> update)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnAttributeUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireAttributesResponse
                {
                    Success = true,
                    Attributes = character!.Attributes
                };
            }
            else
            {
                return new VampireAttributesResponse()
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Skills? skills;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                skills = VampireV5Skills.Load(charId);
                if (skills is not null)
                {
                    return new VampireSkillsResponse
                    {
                        Success = true,
                        Skills = skills,
                    };
                }
            }
            else
            {
                return new VampireSkillsResponse
                {
                    Success = false,
                    Error = "access_error"
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
    public async Task<VampireSkillsResponse> UpdateSkills(int charId, [FromBody] CharacterUpdateWrapper<VampireSkillsUpdate> update)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
                _ = _characterHub.Clients.Group($"character_{charId}")
                    .OnSkillUpdate(charId, update.UpdateId, startTime, update.UpdateData);
                return new VampireSkillsResponse
                {
                    Success = true,
                    Skills = character!.Skills
                };
            }
            else
            {
                return new VampireSkillsResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5SecondaryStats? stats;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                stats = VampireV5SecondaryStats.Load(charId);
                if (stats is not null)
                {
                    return new VampireStatResponse
                    {
                        Success = true,
                        Stats = stats
                    };
                }
            }
            else
            {
                return new VampireStatResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("disciplines/{charId:int}", Name = "Get Character Disciplines")]
    public VampireDisciplinesResponse GetDisciplines(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Disciplines? disciplines;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                disciplines = VampireV5Disciplines.Load(charId);
                if (disciplines is not null)
                {
                    return new VampireDisciplinesResponse
                    {
                        Success = true,
                        Disciplines = disciplines,
                    };
                }
            }
            else
            {
                return new VampireDisciplinesResponse
                {
                    Success = false,
                    Error = "access_error"
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
    public async Task<VampireDisciplinesResponse> UpdateDisciplines(int charId, [FromBody] CharacterUpdateWrapper<VampireDisciplinesUpdate> update)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("powers/{charId:int}", Name = "Get Character Powers")]
    public VampirePowersResponse GetPowers(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5DisciplinePowers? powers;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                powers = VampireV5DisciplinePowers.Load(charId);
                if (powers is not null)
                {
                    return new VampirePowersResponse
                    {
                        Success = true,
                        Powers = powers.GetIdList(),
                    };
                }
            }
            else
            {
                return new VampirePowersResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("beliefs/{charId:int}", Name = "Get Character Beliefs")]
    public VampireBeliefsResponse GetBeliefs(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Beliefs? beliefs;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                beliefs = VampireV5Beliefs.Load(charId);
                if (beliefs is not null)
                {
                    return new VampireBeliefsResponse
                    {
                        Success = true,
                        Beliefs = beliefs,
                    };
                }
            }
            else
            {
                return new VampireBeliefsResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("backgrounds/{charId:int}", Name = "Get Character Backgrounds")]
    public VampireBackgroundMeritFlawResponse GetBackgrounds(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Backgrounds? backgrounds;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                backgrounds = VampireV5Backgrounds.Load(charId);
                if (backgrounds is not null)
                {
                    return new VampireBackgroundMeritFlawResponse
                    {
                        Success = true,
                        Backgrounds = backgrounds,
                    };
                }
            }
            else
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("merits/{charId:int}", Name = "Get Character Merits")]
    public VampireBackgroundMeritFlawResponse GetMerits(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Merits? merits;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                merits = VampireV5Merits.Load(charId);
                if (merits is not null)
                {
                    return new VampireBackgroundMeritFlawResponse
                    {
                        Success = true,
                        Merits = merits,
                    };
                }
            }
            else
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("flaws/{charId:int}", Name = "Get Character Flaws")]
    public VampireBackgroundMeritFlawResponse GetFlaws(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Flaws? flaws;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                flaws = VampireV5Flaws.Load(charId);
                if (flaws is not null)
                {
                    return new VampireBackgroundMeritFlawResponse
                    {
                        Success = true,
                        Flaws = flaws,
                    };
                }
            }
            else
            {
                return new VampireBackgroundMeritFlawResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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

    [HttpGet("profile/{charId:int}", Name = "Get Character Profile")]
    public VampireProfileResponse GetProfile(int charId)
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
        user = UserManager.Instance.GetUser(identity.Name)!;

        VampireV5Profile? profile;
        if (charId > 0)
        {
            // check if allowed!
            if (user.CanViewCharacter(VampireV5Character.GetCharacterHeader(charId)))
            {
                profile = VampireV5Profile.Load(charId);
                if (profile is not null)
                {
                    return new VampireProfileResponse
                    {
                        Success = true,
                        Profile = profile,
                    };
                }
            }
            else
            {
                return new VampireProfileResponse
                {
                    Success = false,
                    Error = "access_error"
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
        user = UserManager.Instance.GetUser(identity.Name)!;

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
            character = VampireV5Character.GetCharacter(charId);
            if (user.CanViewCharacter(character))
            {
                await update.UpdateData.Apply(character!);
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