using Dapper;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.Auth;

public class User {
    public string? Email { get; set; }
    [JsonIgnore]
    public string? Password { get; set; }
    [JsonIgnore]
    public string? ActivationKey { get; set; }
    [JsonIgnore]
    public DateTime? ActivationRequested { get; set; }
    public DateTime? ActivationCompleted { get; set; }
    public bool Active { get; set; }
}

public class UserRescue {
    public int? Id { get; set; }
    public string? Email { get; set; }
    public string? RescueKey { get; set; }
    public DateTime? RescueRequested { get; set; }
    public DateTime? RescueCompleted { get; set; }
}