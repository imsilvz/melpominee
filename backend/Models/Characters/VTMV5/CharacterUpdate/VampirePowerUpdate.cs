using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampirePowerUpdateItem
{
    public string? PowerId { get; set; }
    public bool Remove { get; set; }
}

public class VampirePowersUpdate : ICharacterUpdate
{
    public List<VampirePowerUpdateItem>? PowerIds { get; set; }

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
                "VampirePowersUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        if (PowerIds is not null && PowerIds.Count > 0)
        {
            // convert back to powers list
            var addItems = new List<object>();
            var removeItems = new List<object>();
            foreach (var powerId in PowerIds)
            {
                if (powerId.Remove)
                {
                    removeItems.Add(new
                    {
                        CharId = character.Id,
                        PowerId = powerId.PowerId,
                    });
                }
                else
                {
                    addItems.Add(new
                    {
                        CharId = character.Id,
                        PowerId = powerId.PowerId,
                    });
                }
            }

            // update object reference and save
            using (var conn = DataContext.Instance.Connect())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (removeItems.Count > 0)
                        {
                            string sql =
                            @"
                                DELETE FROM melpominee_character_discipline_powers
                                WHERE charid = @CharId AND powerid = @PowerId;
                            ";
                            await conn.ExecuteAsync(sql, removeItems, transaction: trans);
                        }
                        if (addItems.Count > 0)
                        {
                            string sql =
                            @"
                                INSERT INTO melpominee_character_discipline_powers
                                    (charid, powerid)
                                VALUES
                                    (@CharId, @PowerId)
                                ON CONFLICT(charid, powerid) DO NOTHING;
                            ";
                            await conn.ExecuteAsync(sql, addItems, transaction: trans);
                        }
                        character.DisciplinePowers = await VampireV5DisciplinePowers.Load(conn, trans, (int)character.Id);
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
                var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5DisciplinePowers).Name}";
                await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
            }
        }
    }
}