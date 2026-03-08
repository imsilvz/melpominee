using Dapper;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireBeliefsUpdate : ICharacterUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
    public string? Tenets { get; set; }
    public string? Convictions { get; set; }
    public string? Touchstones { get; set; }

    public async Task Apply(BaseCharacter character, IDistributedCache? cache = null)
    {
        await Apply((VampireV5Character) character, cache);
    }

    public async Task Apply(VampireV5Character character, IDistributedCache? cache = null)
    {
        // Id is required!
        Id = character.Id;
        if (Id is null)
        {
            throw new ArgumentNullException
            (
                "VampireBeliefsUpdate.Apply called with unsaved character. A full save must be performed."
            );
        }

        var updateList = new List<string>();
        if (Tenets is not null)
        {
            character.Beliefs.Tenets = Tenets;
            updateList.Add("Tenets");
        }
        if (Convictions is not null)
        {
            character.Beliefs.Convictions = Convictions;
            updateList.Add("Convictions");
        }
        if (Touchstones is not null)
        {
            character.Beliefs.Touchstones = Touchstones;
            updateList.Add("Touchstones");
        }

        // update object reference and save
        if (updateList.Count < 1) { return; }
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var sql =
                    @$"
                        UPDATE melpominee_character_beliefs
                        SET
                            {String.Join(',', updateList.Select(prop => $"{prop} = @{prop}"))}
                        WHERE CharId = @Id;
                    ";
                    await conn.ExecuteAsync(sql, this, transaction: trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        // handle cache
        if (cache is not null)
        {
            var charKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Character).Name}";
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Beliefs).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}