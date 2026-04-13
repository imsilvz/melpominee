using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Melpominee.app.Authorization;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Characters.VTMV5;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using Melpominee.app.Services.Database;
using Shouldly;
namespace Melpominee.Tests.Unit.Authorization;

public class CanViewCharacterHandlerTests : IAsyncLifetime
{
    private readonly IDistributedCache _cache;
    private readonly UserManager _userManager;
    private readonly CharacterService _characterService;

    public CanViewCharacterHandlerTests()
    {
        _cache = new MemoryDistributedCache(
            Options.Create(new MemoryDistributedCacheOptions()));
        _userManager = new UserManager(_cache, DataContext.Instance);
        _characterService = new CharacterService(_cache, DataContext.Instance);
    }

    public async Task InitializeAsync()
    {
        var owner = new User { Email = "owner@test.com", Role = "user", Active = true };
        await _cache.SetStringAsync(
            "melpominee:user:owner@test.com", JsonSerializer.Serialize(owner));

        var admin = new User { Email = "admin@test.com", Role = "admin", Active = true };
        await _cache.SetStringAsync(
            "melpominee:user:admin@test.com", JsonSerializer.Serialize(admin));

        var stranger = new User { Email = "stranger@test.com", Role = "user", Active = true };
        await _cache.SetStringAsync(
            "melpominee:user:stranger@test.com", JsonSerializer.Serialize(stranger));

        var character = new VampireV5Character { Id = 1, Owner = "owner@test.com" };
        await _cache.SetStringAsync(
            "melpominee:character:1:VampireV5Character", JsonSerializer.Serialize(character));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private (IAuthorizationHandler handler, AuthorizationHandlerContext context) CreateTestContext(
        string email, string role, string? charId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Role, role),
        };
        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext();
        if (charId != null)
        {
            httpContext.Request.RouteValues["charId"] = charId;
        }

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var handler = new CanViewCharacterHandler(_characterService, _userManager, accessor);

        var requirement = new CanViewCharacterRequirement();
        var context = new AuthorizationHandlerContext(
            new[] { requirement }, principal, null);

        return (handler, context);
    }

    [Fact]
    public async Task OwnerCanViewOwnCharacter()
    {
        var (handler, context) = CreateTestContext("owner@test.com", "user", "1");

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeTrue();
    }

    [Fact]
    public async Task AdminCanViewAnyCharacter()
    {
        var (handler, context) = CreateTestContext("admin@test.com", "admin", "1");

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeTrue();
    }

    [Fact]
    public async Task NonOwnerNonAdminCannotView()
    {
        var (handler, context) = CreateTestContext("stranger@test.com", "user", "1");

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
    }

    [Fact]
    public async Task MissingCharIdRouteValue_Fails()
    {
        var (handler, context) = CreateTestContext("owner@test.com", "user", null);

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
    }

    [Fact]
    public async Task CharacterNotInCache_Fails()
    {
        var (handler, context) = CreateTestContext("owner@test.com", "user", "999");

        // charId 999 is not in cache, and DB is unavailable in test —
        // GetCharacterProperty will attempt reflection-based Load which
        // hits DataContext. We expect either null return or an exception,
        // both of which mean the requirement is not satisfied.
        try
        {
            await handler.HandleAsync(context);
            context.HasSucceeded.ShouldBeFalse();
        }
        catch (Exception)
        {
            // DB access failure — handler never called Succeed
            context.HasSucceeded.ShouldBeFalse();
        }
    }
}
