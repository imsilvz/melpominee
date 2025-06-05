using System.Web;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Melpominee.app.Models.Auth;
using Melpominee.app.Models.Web.Auth;
using Melpominee.app.Services;
using Melpominee.app.Services.Auth;
namespace Melpominee.app.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AuthController : ControllerBase
{
    private const string UserKey = "_UserData";
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager _userManager;

    public AuthController(ILogger<AuthController> logger, UserManager userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [ActionName("")]
    [HttpGet(Name = "Index")]
    public async Task<User> Get()
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
            user = await _userManager.GetUser(identity.Name, true);
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
            User? user = await _userManager.Login(payload.Email, payload.Password);
            if (user is not null)
            {
                // create login claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Email!),
                    new Claim(ClaimTypes.Role, user.Role!)
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
                            ClaimTypes.NameIdentifier,
                            ClaimTypes.Role
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
    public async Task<ResetResponse> ResetPassword([FromBody] ResetPayload payload)
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
        if (!await _userManager.BeginResetPassword(payload.Email, $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"))
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
    public async Task<ResetResponse> ResetPasswordConfirm([FromBody] ConfirmResetPayload payload)
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

        if (!await _userManager.FinishResetPassword(payload.Email, payload.Key, payload.Password))
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
    public async Task<RegisterResponse> Register([FromBody] RegisterPayload payload)
    {
        // Check Parameters
        if (string.IsNullOrEmpty(payload.Email) || string.IsNullOrEmpty(payload.Password))
        {
            return new RegisterResponse
            {
                Success = false
            };
        }

        User? user = await _userManager.Register(
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
    public async Task<RedirectResult> RegisterConfirmation([FromQuery] string email, [FromQuery] string activationKey)
    {
        User? user = await _userManager.RegistrationFinish(email, activationKey);
        if (user is null)
        {
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");
        }
        return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/login?notice=confirmed&email={user.Email}");
    }

    private const string DISCORD_AUTHORIZE_URL = "https://discord.com/oauth2/authorize";
    private const string DISCORD_TOKEN_URL = "https://discord.com/api/oauth2/token";
    private const string DISCORD_TOKEN_REVOKE_URL = "https://discord.com/api/oauth2/token/revoke";
    private const string DISCORD_SCOPES = "email identify";
    private const string DISCORD_API_URL = "https://discord.com/api/v10";
    [ActionName("oauth/discord")]
    [HttpGet(Name = "Discord OAuth Flow")]
    public async Task<IActionResult> DiscordOAuth(string? code)
    {
        // if we do not have code, begin the process
        if (code is null)
        {
            var uriBuilder = new UriBuilder(DISCORD_AUTHORIZE_URL);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("response_type", "code");
            parameters.Add("client_id", SecretManager.Instance.GetSecret("discord_clientid"));
            parameters.Add("scope", DISCORD_SCOPES);
            parameters.Add("redirect_uri", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/auth/oauth/discord");
            parameters.Add("prompt", "none");
            uriBuilder.Query = parameters.ToString();
            return Redirect(uriBuilder.Uri.ToString());
        }

        // build token request
        var client = new HttpClient();
        var tokenRequestContent = new Dictionary<string, string?>();
        tokenRequestContent.Add("client_id", SecretManager.Instance.GetSecret("discord_clientid"));
        tokenRequestContent.Add("client_secret", SecretManager.Instance.GetSecret("discord_clientsecret"));
        tokenRequestContent.Add("grant_type", "authorization_code");
        tokenRequestContent.Add("code", code);
        tokenRequestContent.Add("redirect_uri", $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/auth/oauth/discord");
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, DISCORD_TOKEN_URL)
        {
            Content = new FormUrlEncodedContent(tokenRequestContent)
        };

        // make token request
        var tokenResponse = await client.SendAsync(tokenRequest);
        if (!tokenResponse.IsSuccessStatusCode)
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");
        
        // parse token response
        var oauthToken = await tokenResponse.Content.ReadFromJsonAsync<DiscordOAuthTokenResponse>();
        if (oauthToken is null)
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");

        // request user data
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", oauthToken.access_token);
        var userResponse = await client.GetAsync($"{DISCORD_API_URL}/users/@me");
        if (!userResponse.IsSuccessStatusCode)
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");

        // parse user data
        var userData = await userResponse.Content.ReadFromJsonAsync<DiscordOAuthUserResponse>();
        if (userData is null)
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");

        // validate response data
        if (!userData.verified || string.IsNullOrEmpty(userData.email))
            return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/error?message=help");

        // now we can attempt a login!
        var user = await _userManager.GetUser(userData.email, true);
        if (user is null)
        {
            user = await _userManager.OAuthRegister(userData.email, userData.username);
        }
        user.DiscordName = userData.username;
        user.LastLogin = DateTime.UtcNow;
        await _userManager.SaveUser(user);
            
        // create login claims
        var claims = new List<Claim>
        {
            // assert not null as we have performed registration in the previous step
            new Claim(ClaimTypes.NameIdentifier, user!.Email!),
            new Claim(ClaimTypes.Role, user.Role!)
        };

        // store it in the cookie
        await HttpContext.SignInAsync
        (
            "Melpominee.app.Auth.V2",
            new ClaimsPrincipal
            (
                new ClaimsIdentity
                (
                    claims,
                    "Cookies",
                    ClaimTypes.NameIdentifier,
                    ClaimTypes.Role
                )
            )
        );
        HttpContext.Session.Clear();
        return Redirect($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}");
    }
}
