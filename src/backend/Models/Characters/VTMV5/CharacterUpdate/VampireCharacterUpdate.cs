using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireCharacterUpdate : ICharacterUpdate
{
    public string? Name { get; set; }
    public string? Concept { get; set; }
    public string? Chronicle { get; set; }
    public string? Ambition { get; set; }
    public string? Desire { get; set; }
    public string? Sire { get; set; }
    public int? Generation { get; set; }
    public string? Clan { get; set; }
    public string? PredatorType { get; set; }
    public int? Hunger { get; set; }
    public string? Resonance { get; set; }
    public int? BloodPotency { get; set; }
    public int? XpSpent { get; set; }
    public int? XpTotal { get; set; }

    public async Task Apply(BaseCharacter character, IDistributedCache? cache = null)
    {
        await Apply((VampireV5Character) character, cache);
    }

    public async Task Apply(VampireV5Character character, IDistributedCache? cache = null)
    {
        VampireV5Header header;
        var updateList = new List<string>();
        if (Name is not null)
        {
            character.Name = Name;
            updateList.Add("Name");
        }
        if (Concept is not null)
        {
            character.Concept = Concept;
            updateList.Add("Concept");
        }
        if (Chronicle is not null)
        {
            character.Chronicle = Chronicle;
            updateList.Add("Chronicle");
        }
        if (Ambition is not null)
        {
            character.Ambition = Ambition;
            updateList.Add("Ambition");
        }
        if (Desire is not null)
        {
            character.Desire = Desire;
            updateList.Add("Desire");
        }
        if (Sire is not null)
        {
            character.Sire = Sire;
            updateList.Add("Sire");
        }
        if (Generation is not null)
        {
            character.Generation = (int)Generation;
            updateList.Add("Generation");
        }
        if (Clan is not null)
        {
            character.Clan = VampireClan.GetClan(Clan);
            updateList.Add("Clan");
        }
        if (PredatorType is not null)
        {
            character.PredatorType = VampirePredatorType.GetPredatorType(PredatorType);
            updateList.Add("PredatorType");
        }
        if (Hunger is not null)
        {
            character.Hunger = (int)Hunger;
            updateList.Add("Hunger");
        }
        if (Resonance is not null)
        {
            character.Resonance = Resonance;
            updateList.Add("Resonance");
        }
        if (BloodPotency is not null)
        {
            character.BloodPotency = (int)BloodPotency;
            updateList.Add("BloodPotency");
        }
        if (XpSpent is not null)
        {
            character.XpSpent = (int)XpSpent;
            updateList.Add("XpSpent");
        }
        if (XpTotal is not null)
        {
            character.XpTotal = (int)XpTotal;
            updateList.Add("XpTotal");
        }

        // prepare insert
        if (updateList.Count <= 0) { return; }
        header = character.GetHeader();
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var sql =
                    @$"
                        UPDATE melpominee_characters 
                        SET
                            {String.Join(',', updateList.Select(prop => $"{prop} = @{prop}"))}
                        WHERE Id = @Id;
                    ";
                    await conn.ExecuteAsync(sql, header, transaction: trans);
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
            await cache.RemoveAsync(charKey);
        }
    }
}