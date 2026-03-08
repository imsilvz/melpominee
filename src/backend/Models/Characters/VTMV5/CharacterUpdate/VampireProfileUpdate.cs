using Dapper;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireProfileUpdate : ICharacterUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
    public int? TrueAge { get; set; }
    public int? ApparentAge { get; set; }
    public string? DateOfBirth { get; set; }
    public string? DateOfDeath { get; set; }
    public string? Description { get; set; }
    public string? History { get; set; }
    public string? Notes { get; set; }

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
        if (TrueAge is not null)
            updateList.Add("TrueAge");
        if (ApparentAge is not null)
            updateList.Add("ApparentAge");
        if (DateOfBirth is not null)
            updateList.Add("DateOfBirth");
        if (DateOfDeath is not null)
            updateList.Add("DateOfDeath");
        if (Description is not null)
            updateList.Add("Description");
        if (History is not null)
            updateList.Add("History");
        if (Notes is not null)
            updateList.Add("Notes");

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
                        UPDATE melpominee_character_profile
                        SET
                            {String.Join(',', updateList.Select(prop => $"{prop} = @{prop}"))}
                        WHERE charid = @Id;
                    ";
                    await conn.ExecuteAsync(sql, this, transaction: trans);
                    character.Profile = await VampireV5Profile.Load(conn, trans, (int)this.Id);
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
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Profile).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}