using Melpominee.app.Models.Auth;
namespace Melpominee.app.Models.Web.Auth;

public class RegisterPayload 
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class RegisterResponse 
{
    public bool Success { get; set; }
    public User? User { get; set; }
}

public class RegisterConfirmationResponse
{
    public bool Success { get; set; }
}