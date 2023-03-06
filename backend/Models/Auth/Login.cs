namespace Melpominee.app.Models.Auth;

public class LoginPayload 
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginResponse 
{
    public bool Success { get; set; }
    public MelpomineeUser? User { get; set; }
}

public class ResetPayload 
{
    public string? Email { get; set; }
}

public class ResetResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}