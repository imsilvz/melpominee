using Dapper;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melpominee.app.Models.Characters;
namespace Melpominee.app.Models.Auth;

public class User {
    public string? Email { get; set; }
    public string? DiscordName { get; set; } = "";
    [JsonIgnore]
    public string? Password { get; set; }
    public string? Role { get; set; }
    [JsonIgnore]
    public string? ActivationKey { get; set; }
    public DateTime? ActivationRequested { get; set; }
    public DateTime? ActivationCompleted { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool Active { get; set; }

    public bool CanViewCharacter(BaseCharacter? character)
    {
        if (!string.IsNullOrEmpty(character?.Owner) &&
            character.Owner == Email)
        {
            return true;
        }
        return false;
    }
}

public class UserRescue {
    public int? Id { get; set; }
    public string? Email { get; set; }
    public string? RescueKey { get; set; }
    public DateTime? RescueRequested { get; set; }
    public DateTime? RescueCompleted { get; set; }
}