using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Melpominee.app.Models.Auth;
using System.Diagnostics;
namespace Melpominee.app.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
        // Get Active User Data
        MelpomineeUser? user;
        var userString = HttpContext.Session.GetString(UserKey);
        if (string.IsNullOrEmpty(userString)) {
            return new MelpomineeUser();
        } else {
            user = JsonSerializer.Deserialize<MelpomineeUser>(userString);
            if (user is null) {
                return new MelpomineeUser();
            }
            return user;
        }
    }

    [ActionName("Login")]
    [HttpPost(Name = "Login")]
    public LoginResponse Login([FromBody] LoginPayload payload)
    {
        // perform login
        if ((!string.IsNullOrEmpty(payload.Email)) && (!string.IsNullOrEmpty(payload.Password)))
        {
            MelpomineeUser user = new MelpomineeUser(payload.Email);
            if (user.Login(payload.Password))
            {
                HttpContext.Session.SetString(UserKey, JsonSerializer.Serialize(user));
                return new LoginResponse
                {
                    Success = true,
                    User = user,
                };
            }
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

    [ActionName("Register")]
    [HttpPost(Name = "Register")]
    public RegisterResponse Register([FromBody] RegisterPayload payload)
    {
        // Check Parameters
        if (string.IsNullOrEmpty(payload.Email) || string.IsNullOrEmpty(payload.Password))
        {
            return new RegisterResponse
            {
                Success = false
            };
        }

        MelpomineeUser user = new MelpomineeUser(payload.Email);
        if (user.Register($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}", payload.Password)) {
            return new RegisterResponse
            {
                Success = true
            };
        }
        return new RegisterResponse
        {
            Success = false
        };
    }

    [ActionName("Confirmation")]
    [HttpGet(Name = "Register_Confirmation")]
    public RedirectResult RegisterConfirmation([FromQuery] string email, [FromQuery] string activationKey)
    {
        MelpomineeUser user = new MelpomineeUser(email);
        if (user.RegistrationFinish(activationKey))
        {
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/login?confirmed=true&email={user.Email}");
        }
        return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");
    }
}
