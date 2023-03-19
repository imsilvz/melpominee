using Melpominee.app.Models.Auth;
namespace Melpominee.app.Models.Web.Auth;

public class LoginPayload 
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginResponse 
{
    public bool Success { get; set; }
    public User? User { get; set; }
}

public class DiscordOAuthTokenResponse
{
    public string? access_token { get; set; }
    public string? token_type { get; set; }
    public long? expires_in { get; set; }
    public string? refresh_token { get; set; }
    public string? scope { get; set; }
}

// https://discord.com/developers/docs/resources/user
public class DiscordOAuthUserResponse
{
    public string? id { get; set; }
    public string? username { get; set; }
    public string? discriminator { get; set; }
    public string? avatar { get; set; }
    public bool? bot { get; set; }
    public bool? system { get; set; }
    public bool? mfa_enabled { get; set; }
    public string? banner { get; set; }
    public int? accent_color { get; set; }
    public string? locale { get; set; }
    public bool verified { get; set; } = false;
    public string? email { get; set; }
    public int? flags { get; set; }
    public int? premium_type { get; set; }
    public int? public_flags { get; set; }
}