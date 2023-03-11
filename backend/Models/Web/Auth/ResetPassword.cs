namespace Melpominee.app.Models.Web.Auth;

public class ResetPayload 
{
    public string? Email { get; set; }
}

public class ResetResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class ConfirmResetPayload
{
    public string? Key { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}