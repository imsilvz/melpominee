using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireAttributeUpdate : ICharacterUpdate
{
    public int? Strength { get; set; }
    public int? Dexterity { get; set; }
    public int? Stamina { get; set; }
    public int? Charisma { get; set; }
    public int? Manipulation { get; set; }
    public int? Composure { get; set; }
    public int? Intelligence { get; set; }
    public int? Wits { get; set; }
    public int? Resolve { get; set; }

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
                "VampireAttributesUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        // build update query
        var updateList = new List<object>();
        var attrProps = typeof(VampireAttributeUpdate).GetProperties();
        foreach (var attributeProperty in attrProps)
        {
            // skip id
            if (attributeProperty.Name == "Id") { continue; }
            // check if value is null
            var v5Attribute = (int?)attributeProperty.GetValue(this, null);
            if (v5Attribute is not null)
            {
                updateList.Add(new
                {
                    CharId = character.Id,
                    Attr = attributeProperty.Name,
                    Score = v5Attribute
                }
                );
            }
        }

        // don't proceed if there is nothing to update
        if (updateList.Count <= 0) { return; }
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var update =
                    @"
                        INSERT INTO melpominee_character_attributes
                            (charid, attribute, score)
                        VALUES
                            (@CharId, @Attr, @Score)
                        ON CONFLICT(charid, attribute) DO UPDATE 
                        SET
                            score = @Score;
                    ";
                    await conn.ExecuteAsync(update, updateList, transaction: trans);
                    character.Attributes = await VampireV5Attributes.Load(conn, trans, (int)character.Id);
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
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Attributes).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}