using Dapper;
using System.Text.Json.Serialization;
using Melpominee.app.Utilities.Database;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class VampireCharacterResponse
{
    public bool Success { get; set; } = false;
    public string? Error { get; set; }
    public VampireV5Character? Character { get; set; }
}

public class VampireCharacterListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<VampireV5Header>? CharacterList { get; set; }
}

public class VampireCharacterCreateResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int? CharacterId { get; set; }
}

public class VampireHeaderResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Header? Character { get; set; }
}

public class VampireCharacterUpdate
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
    public void Apply(VampireV5Character character)
    {
        character.Name = Name ?? character.Name;
        character.Concept = Concept ?? character.Concept;
        character.Chronicle = Chronicle ?? character.Chronicle;
        character.Ambition = Ambition ?? character.Ambition;
        character.Desire = Desire ?? character.Desire;
        character.Sire = Sire ?? character.Sire;
        character.Generation = Generation ?? character.Generation;
        character.Clan = Clan is not null ? 
            VampireClan.GetClan(Clan) : character.Clan;
        character.PredatorType = PredatorType is not null ? 
            VampirePredatorType.GetPredatorType(PredatorType) : character.PredatorType;
        character.Hunger = Hunger ?? character.Hunger;
        character.Resonance = Resonance ?? character.Resonance;
        character.BloodPotency = BloodPotency ?? character.BloodPotency;
        character.Save();
    }
}

public class VampireAttributesResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Attributes? Attributes { get; set; }
}

public class VampireAttributesUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
    public int? Strength { get; set; }
    public int? Dexterity { get; set; }
    public int? Stamina { get; set; }
    public int? Charisma { get; set; }
    public int? Manipulation { get; set; }
    public int? Composure { get; set; }
    public int? Intelligence { get; set; }
    public int? Wits { get; set; }
    public int? Resolve { get; set; }
    public void Apply(VampireV5Character character)
    {
        // Id is required!
        Id = character.Id;
        if (Id is null) 
        { 
            throw new ArgumentNullException
            (
                "VampireAttributesUpdate.Apply called with unsaved character. A full save must be performed first."
            ); 
        }

        // build update query
        var updateList = new List<object>();
        var attrProps = typeof(VampireAttributesUpdate).GetProperties();
        foreach(var attributeProperty in attrProps)  
        {
            // skip id
            if (attributeProperty.Name == "Id") { continue; }
            // check if value is null
            var v5Attribute = (int?)attributeProperty.GetValue(this, null);
            if (v5Attribute is not null)
            {
                updateList.Add(new 
                    { 
                        CharId = Id, 
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
                            (CharId, Attribute, Score)
                        VALUES
                            (@CharId, @Attr, @Score)
                        ON CONFLICT DO
                        UPDATE SET
                            Score = @Score;
                    ";
                    conn.Execute(update, updateList, transaction: trans);
                    character.Attributes = VampireV5Attributes.Load(conn, trans, (int)Id);
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}

public class VampireSkillsResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Skills? Skills { get; set; }
}

public class VampireSkillsUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
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

    public void Apply(VampireV5Character character)
    {
        // Id is required!
        Id = character.Id;
        if (Id is null) 
        { 
            throw new ArgumentNullException
            (
                "VampireSkillsUpdate.Apply called with unsaved character. A full save must be performed first."
            ); 
        }

        // build update query
        var updateList = new List<object>();
        var skillProps = typeof(VampireSkillsUpdate).GetProperties();
        foreach(var skillProperty in skillProps)  
        {
            // skip id
            if (skillProperty.Name == "Id") { continue; }
            // check if value is null
            var v5Skill = (VampireV5Skill?)skillProperty.GetValue(this, null);
            if (v5Skill is not null)
            {
                updateList.Add(new 
                    { 
                        CharId = Id, 
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
                            (CharId, Skill, Speciality, Score)
                        VALUES
                            (@CharId, @Skill, @Speciality, @Score)
                        ON CONFLICT DO
                        UPDATE SET
                            Speciality = @Speciality,
                            Score = @Score;
                    ";
                    conn.Execute(sql, updateList, transaction: trans);
                    character.Skills = VampireV5Skills.Load(conn, trans, (int)Id);
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }
}

public class VampireDisciplinesResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Disciplines? Disciplines { get; set; }
}

public class VampireDisciplinesUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
    public string? School { get; set; }
    public int Score { get; set; }
    public void Apply(VampireV5Character character)
    {
        // Id is required!
        Id = character.Id;
        if (Id is null) 
        { 
            throw new ArgumentNullException
            (
                "VampireSkillsUpdate.Apply called with unsaved character. A full save must be performed first."
            ); 
        }
        
        var disc = character.Disciplines;
        if (!string.IsNullOrEmpty(School)) 
        {
            using (var conn = DataContext.Instance.Connect())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = "";
                        if (Score <= 0) {
                            sql =
                            @"
                                DELETE FROM melpominee_character_disciplines
                                WHERE CharId = @CharId AND Discipline = @School;
                            ";
                        } else {
                            // make sql query
                            sql =
                            @"
                                INSERT INTO melpominee_character_disciplines
                                    (CharId, Discipline, Score)
                                VALUES
                                    (@CharId, @School, @Score)
                                ON CONFLICT DO
                                UPDATE SET
                                    Score = @Score;
                            ";
                        }
                        conn.Execute(sql, new { CharId = Id, School = School, Score = Score }, transaction: trans);
                        character.Disciplines = VampireV5Disciplines.Load(conn, trans, (int)Id);
                        trans.Commit();
                    }
                    catch(Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}

public class VampirePowersResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<string>? Powers { get; set; }
}

public class VampirePowerUpdateItem
{
    public string? PowerId { get; set; }
    public bool Remove { get; set; }
}

public class VampirePowersUpdate
{
    public int? Id { get; set; }
    public List<VampirePowerUpdateItem>? PowerIds { get; set; }
    public void Apply(VampireV5Character character)
    {
        // Id is required!
        Id = character.Id;
        if (Id is null) 
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
            var newPowers = new VampireV5DisciplinePowers();
            foreach (var powerId in PowerIds)
            {
                if (powerId.Remove) 
                {
                    Console.WriteLine($"REMOVE: {powerId.PowerId}");
                    removeItems.Add(new
                    {
                        CharId = character.Id,
                        PowerId = powerId.PowerId,
                    });
                } 
                else 
                {
                    Console.WriteLine($"ADD: {powerId.PowerId}");
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
                                WHERE CharId = @CharId AND PowerId = @PowerId;
                            ";
                            conn.Execute(sql, removeItems, transaction: trans);
                        }
                        if (addItems.Count > 0)
                        {
                            string sql =
                            @"
                                INSERT OR IGNORE INTO melpominee_character_discipline_powers
                                    (CharId, PowerId)
                                VALUES
                                    (@CharId, @PowerId);
                            ";
                            conn.Execute(sql, addItems, transaction: trans);
                        }
                        character.DisciplinePowers = VampireV5DisciplinePowers.Load(conn, trans, (int)Id);
                        trans.Commit();
                    }
                    catch(Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}