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
    public int? XpSpent { get; set; }
    public int? XpTotal { get; set; }
    public void Apply(VampireV5Character character)
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
                    conn.Execute(sql, header, transaction: trans);
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
    [JsonIgnore]
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

public class VampireBeliefsResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Beliefs? Beliefs { get; set; }
}

public class VampireBeliefsUpdate
{
    [JsonIgnore]
    public int? Id { get; set; }
    public string? Tenets { get; set; }
    public string? Convictions { get; set; }
    public string? Touchstones { get; set; }

    public void Apply(VampireV5Character character)
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
            updateList.Add("Tenets");
        if (Convictions is not null)
            updateList.Add("Convictions");
        if (Touchstones is not null)
            updateList.Add("Touchstones");

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
                    conn.Execute(sql, this, transaction: trans);
                    character.Beliefs = VampireV5Beliefs.Load(conn, trans, (int)this.Id);
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

public class VampireProfileResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Profile? Profile { get; set; }
}

public class VampireProfileUpdate
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

    public void Apply(VampireV5Character character)
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
                        WHERE CharId = @Id;
                    ";
                    conn.Execute(sql, this, transaction: trans);
                    character.Profile = VampireV5Profile.Load(conn, trans, (int)this.Id);
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