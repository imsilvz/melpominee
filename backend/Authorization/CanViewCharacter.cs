using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melpominee.app.Services.Auth;
using Melpominee.app.Models.CharacterSheets.VTMV5;

namespace Melpominee.app.Authorization;

public class CanViewCharacterRequirement : IAuthorizationRequirement { }

public class CanViewCharacterHandler : AuthorizationHandler<CanViewCharacterRequirement>
{
    private readonly UserManager _userManager;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    public CanViewCharacterHandler(UserManager userManager, IHttpContextAccessor httpContextAccessor) 
    { 
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
            Console.WriteLine(httpContext.Request.QueryString);
            Console.WriteLine(String.Join(' ', httpContext.Request.RouteValues.Keys));
            if (user is null || !httpContext.Request.RouteValues.ContainsKey("charId"))
                return;

            // load character data
            int charId = int.Parse((string)httpContext.Request.RouteValues["charId"]!);
            Console.WriteLine(charId);
            var character = VampireV5Character.GetCharacterHeader(charId);
            Console.WriteLine(character?.Loaded);
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
