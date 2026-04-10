using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.Web.VTMV5;

public class CharacterCommandRequest {
  [JsonPropertyName("updateId")]
  public string? UpdateId { get; set; }

  [JsonPropertyName("commands")]
  public List<CharacterCommand>? Commands { get; set; }
}

public class CharacterCommand {
  [JsonPropertyName("type")]
  public string? Type { get; set; }

  [JsonPropertyName("data")]
  public JsonElement? Data { get; set; }
}

public class CharacterCommandResponse {
  [JsonPropertyName("success")]
  public bool Success { get; set; }

  [JsonPropertyName("error")]
  public string? Error { get; set; }
}
