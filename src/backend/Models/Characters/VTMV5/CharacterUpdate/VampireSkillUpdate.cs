using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireSkillUpdate : ICharacterUpdate
{
    public VampireV5Skill? Athletics { get; set; }
    public VampireV5Skill? Brawl { get; set; }
    public VampireV5Skill? Craft { get; set; }
    public VampireV5Skill? Drive { get; set; }
    public VampireV5Skill? Firearms { get; set; }
    public VampireV5Skill? Melee { get; set; }
    public VampireV5Skill? Larceny { get; set; }
    public VampireV5Skill? Stealth { get; set; }
    public VampireV5Skill? Survival { get; set; }
    public VampireV5Skill? AnimalKen { get; set; }
    public VampireV5Skill? Ettiquette { get; set; }
    public VampireV5Skill? Insight { get; set; }
    public VampireV5Skill? Intimidation { get; set; }
    public VampireV5Skill? Leadership { get; set; }
    public VampireV5Skill? Performance { get; set; }
    public VampireV5Skill? Persuasion { get; set; }
    public VampireV5Skill? Streetwise { get; set; }
    public VampireV5Skill? Subterfuge { get; set; }
    public VampireV5Skill? Academics { get; set; }
    public VampireV5Skill? Awareness { get; set; }
    public VampireV5Skill? Finance { get; set; }
    public VampireV5Skill? Investigation { get; set; }
    public VampireV5Skill? Medicine { get; set; }
    public VampireV5Skill? Occult { get; set; }
    public VampireV5Skill? Politics { get; set; }
    public VampireV5Skill? Science { get; set; }
    public VampireV5Skill? Technology { get; set; }

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

        // build update query
        var updateList = new List<object>();
        var skillProps = typeof(VampireSkillUpdate).GetProperties();
        foreach (var skillProperty in skillProps)
        {
            // skip id
            if (skillProperty.Name == "Id") { continue; }
            // check if value is null
            var v5Skill = (VampireV5Skill?)skillProperty.GetValue(this, null);
            if (v5Skill is not null)
            {
                // set new value for our character
                typeof(VampireV5Skills).GetProperty(skillProperty.Name)!
                    .SetValue(character.Skills, v5Skill);
                updateList.Add(new
                {
                    CharId = character.Id,
                    Skill = skillProperty.Name,
                    Speciality = v5Skill.Speciality,
                    Score = v5Skill.Score,
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
                    var sql =
                    @"
                        INSERT INTO melpominee_character_skills
                            (charid, skill, speciality, score)
                        VALUES
                            (@CharId, @Skill, @Speciality, @Score)
                        ON CONFLICT(charid, skill) DO UPDATE 
                        SET
                            speciality = @Speciality,
                            score = @Score;
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

        // handle cache
        if (cache is not null)
        {
            var charKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Character).Name}";
            var propKey = $"melpominee:character:{character.Id}:{typeof(VampireV5Skills).Name}";
            await Task.WhenAll(new Task[] { cache.RemoveAsync(charKey), cache.RemoveAsync(propKey) } );
        }
    }
}