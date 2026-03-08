using Dapper;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireDisciplineUpdate : ICharacterUpdate
{
    // JsonPropertyName to enforce case
    // case is important for this particular API
    // as these properties might be used as keys!
    [JsonPropertyName("Animalism")]
    public int? Animalism { get; set; }
    [JsonPropertyName("Auspex")]
    public int? Auspex { get; set; }
    [JsonPropertyName("BloodSorcery")]
    public int? BloodSorcery { get; set; }
    [JsonPropertyName("Celerity")]
    public int? Celerity { get; set; }
    [JsonPropertyName("Dominate")]
    public int? Dominate { get; set; }
    [JsonPropertyName("Fortitude")]
    public int? Fortitude { get; set; }
    [JsonPropertyName("Obfuscate")]
    public int? Obfuscate { get; set; }
    [JsonPropertyName("Oblivion")]
    public int? Oblivion { get; set; }
    [JsonPropertyName("Potence")]
    public int? Potence { get; set; }
    [JsonPropertyName("Presence")]
    public int? Presence { get; set; }
    [JsonPropertyName("Protean")]
    public int? Protean { get; set; }
    [JsonPropertyName("ThinBloodAlchemy")]
    public int? ThinBloodAlchemy { get; set; }

    public async Task Apply(BaseCharacter character, IDistributedCache? cache = null)
    {
        await Apply((VampireV5Character) character, cache);
    }

    public async Task Apply(VampireV5Character character, IDistributedCache? cache = null)
    {
        // Id is required!
        if (character.Id is null)
        {
            throw new ArgumentNullException
            (
                "VampireSkillsUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        // create update listings
        var disc = character.Disciplines;
        List<object> updateList = new List<object>();
        foreach(var prop in typeof(VampireDisciplineUpdate).GetProperties())
        {
            if(prop.Name == "CharId") { continue; }
            var Score = (int?)prop.GetValue(this);
            if(Score is not null)
            {
                if(Score > 0)
                    disc[prop.Name] = (int)Score;
                else
                    disc.Remove(prop.Name);
                updateList.Add(new { CharId = character.Id, School = prop.Name, Score = Score });
            }
        }

        // handle query
        if (updateList.Count > 0)
        {
            using (var conn = DataContext.Instance.Connect())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql =
                        @"
                            INSERT INTO melpominee_character_disciplines
                                (charid, discipline, score)
                            VALUES
                                (@CharId, @School, @Score)
                            ON CONFLICT(charid, discipline) DO UPDATE 
                            SET
                                score = @Score;
                            DELETE FROM melpominee_character_disciplines
                            WHERE charid = @CharId 
                                AND discipline = @School 
                                AND score <= 0;
                        ";
                        await conn.ExecuteAsync(sql, updateList, transaction: trans);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        // handle cache
        if (cache is not null)
        {
            var charKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Character).Name}";
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Disciplines).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}