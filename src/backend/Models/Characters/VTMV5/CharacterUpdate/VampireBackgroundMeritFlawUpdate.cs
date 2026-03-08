using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireBackgroundMeritFlawUpdate : ICharacterUpdate
{
    public VampireV5BackgroundMeritFlaw? Backgrounds { get; set; }
    public VampireV5BackgroundMeritFlaw? Merits { get; set; }
    public VampireV5BackgroundMeritFlaw? Flaws { get; set; }

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
                "VampireBackgroundMeritFlawUpdate.Apply called with unsaved character. A full save must be performed."
            );
        }

        if (Backgrounds is not null)
            await Apply(character, VampireV5Backgrounds.ItemType, Backgrounds);
        if (Merits is not null)
            await Apply(character, VampireV5Merits.ItemType, Merits);
        if (Flaws is not null)
            await Apply(character, VampireV5Flaws.ItemType, Flaws);

        // handle cache
        if (cache is not null)
        {
            var charKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Character).Name}";
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Backgrounds).Name}";
            var prop2Key = $"melpominee:character:{character.Id}:{typeof(VampireV5Merits).Name}";
            var prop3Key = $"melpominee:character:{character.Id}:{typeof(VampireV5Flaws).Name}";
            await Task.WhenAll(new Task[] 
                { 
                    cache.RemoveAsync(charKey), 
                    cache.RemoveAsync(propKey), 
                    cache.RemoveAsync(prop2Key), 
                    cache.RemoveAsync(prop3Key) 
                } 
            );
        }
    }

    private async Task Apply(VampireV5Character character, string ItemType, VampireV5BackgroundMeritFlaw Items)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // set update fields
                    foreach (var item in Items.Values)
                    {
                        item.CharId = character.Id;
                        item.ItemType = ItemType;
                    };

                    var sql =
                    @"
                        INSERT INTO melpominee_character_meritflawbackgrounds
                            (charid, itemtype, sortorder, name, score)
                        VALUES
                            (@CharId, @ItemType, @SortOrder, @Name, @Score)
                        ON CONFLICT(charid, itemtype, sortorder) DO UPDATE
                        SET
                            name = @Name,
                            score = @Score;
                    ";
                    await conn.ExecuteAsync(sql, Items.Values, transaction: trans);
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
}