namespace Melpominee.app.Models.Auth;

public class RegisterPayload 
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class RegisterResponse 
{
    public bool Success { get; set; }
    public MelpomineeUser? User { get; set; }
}

public class RegisterConfirmationResponse
{
    public bool Success { get; set; }
}