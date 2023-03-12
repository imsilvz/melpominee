using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;
using Melpominee.app.Utilities.Database;

namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Character : BaseCharacterSheet
{
    // meta
    [JsonIgnore]
    public bool Loaded = false;

    // header items
    public string Name { get; set; } = "";
    public string Concept { get; set; } = "";
    public string Chronicle { get; set; } = ""; // inherit from GameId
    public string Ambition { get; set; } = "";
    public string Desire { get; set; } = "";
    public string Sire { get; set; } = "";
    public int Generation { get; set; } = 13;
    [JsonConverter(typeof(VampireClanJsonConverter))]
    public VampireClan? Clan { get; set; }
    [JsonConverter(typeof(VampirePredatorTypeJsonConverter))]
    public VampirePredatorType? PredatorType { get; set; }

    // primary stats
    public VampireV5Attributes Attributes { get; set; } = new VampireV5Attributes();
    public VampireV5Skills Skills { get; set; } = new VampireV5Skills();
    public VampireV5SecondaryStats SecondaryStats { get; set; } = new VampireV5SecondaryStats();

    // disciplines
    public VampireV5Disciplines Disciplines { get; set; } = new VampireV5Disciplines();
    [JsonConverter(typeof(VampirePowerListJsonConverter))]
    public VampireV5DisciplinePowers DisciplinePowers { get; set; } = new VampireV5DisciplinePowers();

    // additional stat tracks
    public int Hunger { get; set; } = 0;
    public string Resonance { get; set; } = "";
    public int BloodPotency { get; set; } = 0;

    public VampireV5Character() : base()
    {
        GameType = "VTMV5";
    }

    public override bool Save()
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var sql = "";
                    if (Id is null)
                    {
                        // we must create one!
                        sql =
                        @"
                            INSERT INTO melpominee_characters
                                (
                                    Name, Concept, Chronicle, 
                                    Ambition, Desire, Sire, 
                                    Generation, Clan, PredatorType,
                                    Hunger, Resonance, BloodPotency
                                )
                            VALUES
                                (
                                    @Name, @Concept, @Chronicle,
                                    @Ambition, @Desire, @Sire,
                                    @Generation, @Clan, @PredatorType,
                                    @Hunger, @Resonance, @BloodPotency
                                )
                            RETURNING Id;
                        ";
                        Id = conn.ExecuteScalar<int>(sql, this);
                    } else {
                        sql =
                        @"
                            INSERT INTO melpominee_characters
                                (
                                    Id, Name, Concept, Chronicle, 
                                    Ambition, Desire, Sire, 
                                    Generation, Clan, PredatorType,
                                    Hunger, Resonance, BloodPotency
                                )
                            VALUES
                                (
                                    @Id, @Name, @Concept, @Chronicle,
                                    @Ambition, @Desire, @Sire,
                                    @Generation, @Clan, @PredatorType,
                                    @Hunger, @Resonance, @BloodPotency
                                )
                            ON CONFLICT DO
                            UPDATE SET
                                Name = @Name,
                                Concept = @Concept,
                                Chronicle = @Chronicle,
                                Ambition = @Ambition,
                                Desire = @Desire,
                                Sire = @Sire,
                                Generation = @Generation,
                                Clan = @Clan,
                                PredatorType = @PredatorType,
                                Hunger = @Hunger,
                                Resonance = @Resonance,
                                BloodPotency = @BloodPotency
                            RETURNING Id;
                        ";
                        conn.ExecuteScalar<int>(sql, this);
                    }
                    Attributes.Save(conn, (int) Id);
                    Skills.Save(conn, (int)Id);
                    SecondaryStats.Save(conn, (int)Id);
                    Disciplines.Save(conn, (int)Id);
                    DisciplinePowers.Save(conn, (int)Id);
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        return true;
    }

    public static VampireV5Character? GetCharacter(int id)
    {
        // make connection
        VampireV5Character? user;
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                var sql =
                @"
                    SELECT
                        Id, Name, Concept, Chronicle, 
                        Ambition, Desire, Sire, 
                        Generation, Clan, PredatorType,
                        Hunger, Resonance, BloodPotency
                    FROM melpominee_characters
                    WHERE Id = @Id;
                ";
                user = conn.QuerySingleOrDefault<VampireV5Character>(sql, new { Id = id });

                // periodically check status
                if (user is null)
                {
                    return null;
                }

                // fetch dependant objects
                var attributes = VampireV5Attributes.Load(conn, id);
                var skills = VampireV5Skills.Load(conn, id);
                var secondaryStats = VampireV5SecondaryStats.Load(conn, id);
                var disciplines = VampireV5Disciplines.Load(conn, id);
                var disciplinePowers = VampireV5DisciplinePowers.Load(conn, id);

                // if any fail to load, keep defaults
                user.Attributes = attributes ?? user.Attributes;
                user.Skills = skills ?? user.Skills;
                user.SecondaryStats = secondaryStats ?? user.SecondaryStats;
                user.Disciplines = disciplines ?? user.Disciplines;
                user.DisciplinePowers = disciplinePowers ?? user.DisciplinePowers;
            }
        }
        user.Loaded = true;
        return user;
    }
}

public class VampireV5Attributes
{
    public int Strength { get; set; } = 0;
    public int Dexterity { get; set; } = 0;
    public int Stamina { get; set; } = 0;
    public int Charisma { get; set; } = 0;
    public int Manipulation { get; set; } = 0;
    public int Composure { get; set; } = 0;
    public int Intelligence { get; set; } = 0;
    public int Wits { get; set; } = 0;
    public int Resolve { get; set; } = 0;

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, charId);
                    trans.Commit();
                    return res;
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var attrPropList = typeof(VampireV5Attributes).GetProperties();
        for(int i=0; i<attrPropList.Length; i++)  
        {
            var attributeProperty = attrPropList[i];
            var v5Attribute = (int)attributeProperty.GetValue(this, null)!;
            rowList.Add(new { CharId = charId, Attr = attributeProperty.Name, Score = v5Attribute});
        }

        // make sql query
        var sql =
        @"
            INSERT INTO melpominee_character_attributes
                (CharId, Attribute, Score)
            VALUES
                (@CharId, @Attr, @Score)
            ON CONFLICT DO
            UPDATE SET
                Score = @Score;
        ";
        conn.Execute(sql, rowList);
        return true;
    }

    public static VampireV5Attributes Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            return Load(conn, charId);
        }
    }

    public static VampireV5Attributes Load(IDbConnection conn, int charId)
    {
        VampireV5Attributes attr = new VampireV5Attributes();
        var sql =
        @"
            SELECT Attribute, Score
            FROM melpominee_character_attributes
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId });
        foreach(var result in results)
        {
            Type attrType = typeof(VampireV5Attributes);                   
            var propInfo = attrType.GetProperty(result.Attribute);
            propInfo?.SetValue(attr, (int)result.Score, null);
        }
        return attr;
    }
}

public class VampireV5Skill
{
    public string Speciality { get; set; } = "";
    public int Score { get; set; } = 0;
}

public class VampireV5Skills
{
    public VampireV5Skill Athletics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Brawl { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Craft { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Drive { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Firearms { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Melee { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Larceny { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Stealth { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Survival { get; set; } = new VampireV5Skill {};
    public VampireV5Skill AnimalKen { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Ettiquette { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Insight { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Intimidation { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Leadership { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Performance { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Persuasion { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Streetwise { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Subterfuge { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Academics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Awareness { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Finance { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Investigation { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Medicine { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Occult { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Politics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Science { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Technology { get; set; } = new VampireV5Skill {};

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, charId);
                    trans.Commit();
                    return res;
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var skillPropList = typeof(VampireV5Skills).GetProperties();
        for(int i=0; i<skillPropList.Length; i++)  
        {
            var skillProperty = skillPropList[i];
            var v5Skill = (VampireV5Skill)skillProperty.GetValue(this, null)!;
            rowList.Add(new { 
                CharId = charId, 
                Skill = skillProperty.Name, 
                Speciality = v5Skill.Speciality,
                Score = v5Skill.Score,
            });
        }

        // make sql query
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
        conn.Execute(sql, rowList);
        return true;
    }

    public static VampireV5Skills Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            return Load(conn, charId);
        }
    }

    public static VampireV5Skills Load(IDbConnection conn, int charId)
    {
        VampireV5Skills skills = new VampireV5Skills();
        var sql =
        @"
            SELECT Skill, Speciality, Score
            FROM melpominee_character_skills
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId });
        foreach(var result in results)
        {
            VampireV5Skill skill = new VampireV5Skill()
            {
                Speciality = (string)result.Speciality,
                Score = (int)result.Score,
            };

            Type skillType = typeof(VampireV5Skills);                   
            var propInfo = skillType.GetProperty(result.Skill);
            propInfo?.SetValue(skills, skill, null);
        }
        return skills;
    }
}

public class VampireV5SecondaryStat
{
    public int BaseValue { get; set; } = 0;
    public int SuperficialDamage { get; set; } = 0;
    public int AggravatedDamage { get; set; } = 0;
}

public class VampireV5SecondaryStats
{
    public VampireV5SecondaryStat Health { get; set; } = new VampireV5SecondaryStat();
    public VampireV5SecondaryStat Willpower { get; set; } = new VampireV5SecondaryStat();
    public VampireV5SecondaryStat Humanity { get; set; } = new VampireV5SecondaryStat
    {
        BaseValue = 7,
    };

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, charId);
                    trans.Commit();
                    return res;
                }
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var statPropList = typeof(VampireV5SecondaryStats).GetProperties();
        for(int i=0; i<statPropList.Length; i++)  
        {
            var statProperty = statPropList[i];
            var v5Stat = (VampireV5SecondaryStat)statProperty.GetValue(this, null)!;
            rowList.Add(new { 
                CharId = charId, 
                Stat = statProperty.Name,
                BaseValue = v5Stat.BaseValue,
                SuperficialDamage = v5Stat.SuperficialDamage,
                AggravatedDamage = v5Stat.AggravatedDamage,
            });
        }

        // make sql query
        var sql =
        @"
            INSERT INTO melpominee_character_secondary
                (
                    CharId, StatName, BaseValue, 
                    SuperficialDamage, AggravatedDamage
                )
            VALUES
                (
                    @CharId, @Stat, @BaseValue, 
                    @SuperficialDamage, @AggravatedDamage)
            ON CONFLICT DO
            UPDATE SET
                BaseValue = @BaseValue,
                SuperficialDamage = @SuperficialDamage,
                AggravatedDamage = @AggravatedDamage;
        ";
        conn.Execute(sql, rowList);
        return true;
    }

    public static VampireV5SecondaryStats? Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            return Load(conn, charId);
        }
    }

    public static VampireV5SecondaryStats? Load(IDbConnection conn, int charId)
    {
        VampireV5SecondaryStats stats = new VampireV5SecondaryStats();
        var sql =
        @"
            SELECT 
                StatName, BaseValue,
                SuperficialDamage, AggravatedDamage
            FROM melpominee_character_secondary
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId });
        foreach(var result in results)
        {
            VampireV5SecondaryStat stat = new VampireV5SecondaryStat()
            {
                BaseValue = (int)result.BaseValue,
                SuperficialDamage = (int)result.SuperficialDamage,
                AggravatedDamage = (int)result.AggravatedDamage,
            };

            Type statsType = typeof(VampireV5SecondaryStats);                   
            var propInfo = statsType.GetProperty(result.StatName);
            propInfo?.SetValue(stats, stat, null);
        }
        return stats;
    }
}