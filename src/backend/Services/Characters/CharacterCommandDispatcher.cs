using System.Collections.Frozen;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Models.Characters;
using Melpominee.app.Models.Characters.VTMV5;
using Melpominee.app.Models.Web.VTMV5;
namespace Melpominee.app.Services.Characters;

public class CharacterCommandDispatcher {
  // Match ASP.NET Core's web defaults so camelCase JSON from the
  // frontend correctly binds to PascalCase DTO properties.
  private static readonly JsonSerializerOptions DeserializeOptions = new() {
    PropertyNameCaseInsensitive = true
  };

  private static readonly FrozenDictionary<string, Type> CommandMap =
    new Dictionary<string, Type> {
      ["header"] = typeof(VampireCharacterUpdate),
      ["attributes"] = typeof(VampireAttributeUpdate),
      ["skills"] = typeof(VampireSkillUpdate),
      ["stats"] = typeof(VampireStatsUpdate),
      ["disciplines"] = typeof(VampireDisciplineUpdate),
      ["powers"] = typeof(VampirePowersUpdate),
      ["beliefs"] = typeof(VampireBeliefsUpdate),
      ["backgrounds"] = typeof(VampireBackgroundMeritFlawUpdate),
      ["merits"] = typeof(VampireBackgroundMeritFlawUpdate),
      ["flaws"] = typeof(VampireBackgroundMeritFlawUpdate),
      ["profile"] = typeof(VampireProfileUpdate),
    }.ToFrozenDictionary();

  private readonly IDistributedCache _cache;

  public CharacterCommandDispatcher(IDistributedCache cache) {
    _cache = cache;
  }

  public async Task<List<CharacterCommand>> ApplyCommands(
      VampireV5Character character,
      List<CharacterCommand> commands) {
    var applied = new List<CharacterCommand>();
    foreach (var cmd in commands) {
      if (cmd.Type is null || cmd.Data is null) {
        continue;
      }

      if (!CommandMap.TryGetValue(cmd.Type, out var dtoType)) {
        throw new ArgumentException(
          $"Unknown command type: {cmd.Type}");
      }

      var dto = (ICharacterUpdate?)cmd.Data.Value
        .Deserialize(dtoType, DeserializeOptions);
      if (dto is null) {
        continue;
      }

      await dto.Apply(character, _cache);
      applied.Add(cmd);
    }
    return applied;
  }
}
