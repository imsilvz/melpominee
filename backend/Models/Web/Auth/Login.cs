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