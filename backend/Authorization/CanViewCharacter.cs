using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using Melpominee.app.Models.Characters.VTMV5;

namespace Melpominee.app.Authorization;

public class CanViewCharacterRequirement : IAuthorizationRequirement { }

public class CanViewCharacterHandler : AuthorizationHandler<CanViewCharacterRequirement>
{
    private readonly CharacterService _characterService;
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    public CanViewCharacterHandler(
        CharacterService characterService, 
        UserManager userManager, 
        IHttpContextAccessor httpContextAccessor) 
    { 
        _characterService = characterService;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanViewCharacterRequirement requirement)
    {
        HttpContext? httpContext = _httpContextAccessor?.HttpContext;
        var userIdentity = context.User.Identities.First();
        var userId = userIdentity.Name;

        if (userIdentity.IsAuthenticated && httpContext is not null)
        {
            var user = await _userManager.GetUser(userId);

            // check to confirm that we have the desired parameter
            if (user is null || !httpContext.Request.RouteValues.ContainsKey("charId"))
                return;

            // load character data
            int charId = int.Parse((string)httpContext.Request.RouteValues["charId"]!);
            var character = _characterService.GetCharacterProperty<VampireV5Character>(charId);
            Console.WriteLine(character is null || !character.Loaded);
            if (character is null || !character.Loaded)
                return;

            // check ownership
            if (user.Email == character.Owner) 
            {
                context.Succeed(requirement);
            }
        }
    }
}
