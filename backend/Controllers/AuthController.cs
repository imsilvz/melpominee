using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Web.Auth;
using Melpominee.app.Utilities.Auth;
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
    public User Get()
    {
        var identity = HttpContext.User.Identity;
        if (identity is null)
        {
            return new User();
        }
        else
        {
            User? user;
            if (!identity.IsAuthenticated)
            {
                return new User();
            }
            user = UserManager.Instance.GetUser(identity.Name, true);
            return user is null ? new User() : user;
        }
    }

    [ActionName("login")]
    [HttpPost(Name = "Login")]
    public async Task<LoginResponse> Login([FromBody] LoginPayload payload)
    {
        // perform login
        if (!(string.IsNullOrEmpty(payload.Email) || string.IsNullOrEmpty(payload.Password)))
        {
            User? user = UserManager.Instance.Login(payload.Email, payload.Password);
            if (user is not null)
            {
                // create login claims
                var claims = new List<Claim>
                {
                    new Claim("user", user.Email!),
                    new Claim("role", "user")
                };

                // store it in the session
                await HttpContext.SignInAsync
                (
                    "Melpominee.app.Auth",
                    new ClaimsPrincipal
                    (
                        new ClaimsIdentity
                        (
                            claims,
                            "Cookies",
                            "user",
                            "role"
                        )
                    )
                );
                HttpContext.Session.Clear();

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

    [ActionName("logout")]
    [HttpGet(Name = "Logout")]
    [HttpPost(Name = "Logout")]
    public async Task<bool> Logout()
    {
        await HttpContext.SignOutAsync();
        HttpContext.Session.Clear();
        return true;
    }

    [ActionName("reset-password")]
    [HttpPost(Name = "Reset_Password")]
    public ResetResponse ResetPassword([FromBody] ResetPayload payload)
    {
        if (string.IsNullOrEmpty(payload.Email))
        {
            return new ResetResponse
            {
                Success = false,
                Error = "missing_email"
            };
        }

        // trigger reset password logic
        if (!UserManager.Instance.BeginResetPassword(payload.Email, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"))
        {
            return new ResetResponse
            {
                Success = false,
                Error = "not_found"
            };
        }

        return new ResetResponse
        {
            Success = true
        };
    }

    [ActionName("reset-password/confirmation")]
    [HttpPost(Name = "Reset_Password_Confirm")]
    public ResetResponse ResetPasswordConfirm([FromBody] ConfirmResetPayload payload)
    {
        if (string.IsNullOrEmpty(payload.Key)
            || string.IsNullOrEmpty(payload.Email)
            || string.IsNullOrEmpty(payload.Password))
        {
            return new ResetResponse
            {
                Success = false,
                Error = "missing_params"
            };
        }

        if (!UserManager.Instance.FinishResetPassword(payload.Email, payload.Key, payload.Password))
        {
            return new ResetResponse
            {
                Success = false,
                Error = "generic"
            };
        }
        return new ResetResponse
        {
            Success = true
        };
    } 

    [ActionName("register")]
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

        User? user = UserManager.Instance.Register(
            payload.Email, 
            payload.Password,
            $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
        );
        if (user is not null)
        {
            return new RegisterResponse
            {
                Success = true,
                User = user,
            };
        }
        return new RegisterResponse
        {
            Success = false
        };
    }

    [ActionName("register/confirmation")]
    [HttpGet(Name = "Register_Confirmation")]
    public RedirectResult RegisterConfirmation([FromQuery] string email, [FromQuery] string activationKey)
    {
        User? user = UserManager.Instance.RegistrationFinish(email, activationKey);
        if (user is null)
        {
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");
        }
        return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/login?notice=confirmed&email={user.Email}");
    }
}
