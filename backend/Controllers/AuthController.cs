using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Melpominee.app.Models.Auth;

namespace Melpominee.app.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private const string UserKey = "_UserData";
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [ActionName("")]
    [HttpGet(Name = "Index")]
    public MelpomineeUser Get()
    {
        MelpomineeUser? user;
        var userString = HttpContext.Session.GetString(UserKey);
        if (string.IsNullOrEmpty(userString)) {
            return new MelpomineeUser
            {
                Email = "",
            };
        } else {
            user = JsonSerializer.Deserialize<MelpomineeUser>(userString);
            if (user is null) {
                return new MelpomineeUser
                {
                    Email = "",
                };
            }
            return user;
        }
    }

    [ActionName("Login")]
    [HttpPost(Name = "Login")]
    public LoginResponse Login([FromBody] LoginPayload payload)
    {
        Console.WriteLine(payload.Email);
        Console.WriteLine(payload.Password);
        if ((!string.IsNullOrEmpty(payload.Email)) && (!string.IsNullOrEmpty(payload.Password)))
        {
            MelpomineeUser user = new MelpomineeUser
            {
                Email = payload.Email
            };

            HttpContext.Session.SetString(UserKey, JsonSerializer.Serialize(user));
            return new LoginResponse
            {
                Success = true,
                User = user,
            };
        }
        return new LoginResponse
        {
            Success = false,
        };
    }

    [ActionName("Logout")]
    [HttpGet(Name = "Logout")]
    [HttpPost(Name = "Logout")]
    public bool Logout()
    {
        HttpContext.Session.Clear();
        return true;
    }
}
