using Dapper;
using System.Data;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireStatUpdate
{
    [JsonIgnore]
    public int? CharId { get; set; }
    [JsonIgnore]
    public string? StatName { get; set; }
    public int? BaseValue { get; set; }
    public int? SuperficialDamage { get; set; }
    public int? AggravatedDamage { get; set; }

    public async Task Apply(IDbConnection conn, IDbTransaction trans, VampireV5Character character, string statName)
    {
        // Id is required!
        CharId = character.Id;
        StatName = statName;
        if (CharId is null)
        {
            throw new ArgumentNullException
            (
                "VampireStatUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        // grab object thru reflection
        VampireV5SecondaryStat statObject = (VampireV5SecondaryStat)typeof(VampireV5SecondaryStats)
            .GetProperty(statName)!
            .GetValue(character.SecondaryStats)!;

        // make changes
        List<string> updList = new List<string>();
        if (BaseValue is not null)
        {
            updList.Add("BaseValue");
            statObject.BaseValue = (int)BaseValue;
        }
        if (SuperficialDamage is not null)
        {
            updList.Add("SuperficialDamage");
            statObject.SuperficialDamage = (int)SuperficialDamage;
        }
        if (AggravatedDamage is not null)
        {
            updList.Add("AggravatedDamage");
            statObject.AggravatedDamage = (int)AggravatedDamage;
        }

        var sql =
        @$"
            UPDATE melpominee_character_secondary
            SET
                {String.Join(", ", updList.Select(i => $"{i} = @{i}"))}
            WHERE charid = @CharId AND statname = @StatName;
        ";
        await conn.ExecuteAsync(sql, this, transaction: trans);
    }
}

public class VampireStatsUpdate : ICharacterUpdate
{
    public VampireStatUpdate? Health { get; set; }
    public VampireStatUpdate? Willpower { get; set; }
    public VampireStatUpdate? Humanity { get; set; }

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
                "VampireStatsUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    if (Health is not null)
                        await Health.Apply(conn, trans, character, "Health");
                    if (Willpower is not null)
                        await Willpower.Apply(conn, trans, character, "Willpower");
                    if (Humanity is not null)
                        await Humanity.Apply(conn, trans, character, "Humanity");
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
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5SecondaryStats).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}