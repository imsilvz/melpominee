using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;
using Melpominee.app.Utilities.Database;

namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Sheet : BaseCharacterSheet
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

    public VampireV5Sheet() : base()
    {
        GameType = "VTMV5";
    }

    public VampireV5Sheet(int id) : base(id)
    {
        GameType = "VTMV5";
    }

    public override bool Load()
    {
        // check that we have something to load
        if (Id is null) { return false; }

        /*using(var connection = new SqliteConnection("Data Source=data/melpominee.db"))
        using(var cmd = connection.CreateCommand())
        {
            // open connection
            connection.Open();

            // load data
            cmd.Parameters.AddWithValue("$id", Id);
            cmd.CommandText =
            @"
                SELECT 
                    name, concept, chronicle,
                    ambition, desire, sire,
                    generation, clan, predator_type,
                    hunger, resonance, blood_potency
                FROM melpominee_characters
                WHERE id = $id;
                SELECT attribute, score
                FROM melpominee_character_attributes
                WHERE sheet_id = $id;
                SELECT skill, speciality, score
                FROM melpominee_character_skills
                WHERE sheet_id = $id;
                SELECT stat_name, base_value, superficial_damage, aggravated_damage
                FROM melpominee_character_secondary
                WHERE sheet_id = $id;
                SELECT discipline, score
                FROM melpominee_character_disciplines
                WHERE sheet_id = $id;
                SELECT power_name
                FROM melpominee_character_discipline_powers
                WHERE sheet_id = $id;
            ";
            using(var reader = cmd.ExecuteReader())
            {
                // read header data
                while(reader.Read())
                {
                    string clanKey, predatorKey;
                    Name = reader.GetString(0);
                    Concept = reader.GetString(1);
                    Chronicle = reader.GetString(2);
                    Ambition = reader.GetString(3);
                    Desire = reader.GetString(4);
                    Sire = reader.GetString(5);
                    Generation = reader.GetInt32(6);
                    clanKey = reader.GetString(7);
                    predatorKey = reader.GetString(8);
                    Clan = VampireClan.GetClan(clanKey);
                    PredatorType = VampirePredatorType.GetPredatorType(predatorKey);
                    Hunger = reader.GetInt32(9);
                    Resonance = reader.GetString(10);
                    BloodPotency = reader.GetInt32(11);
                }

                // read attribute data
                reader.NextResult();
                while(reader.Read())
                {
                    string attributeName = reader.GetString(0);
                    int attributeScore = reader.GetInt32(1);
                    var attributeProperty = typeof(VampireV5Attributes).GetProperty(attributeName);
                    if (attributeProperty is not null)
                    {
                        attributeProperty.SetValue(this.Attributes, attributeScore);
                    }
                }

                // read skill data
                reader.NextResult();
                while(reader.Read())
                {
                    string skillName = reader.GetString(0);
                    string skillSpeciality = reader.GetString(1);
                    int skillScore = reader.GetInt32(2);
                    var skillProperty = (VampireV5Skill?)typeof(VampireV5Skills)
                        .GetProperty(skillName)?
                        .GetValue(this.Skills, null);
                    if(skillProperty is not null)
                    {
                        skillProperty.Speciality = skillSpeciality;
                        skillProperty.Score = skillScore;
                    }
                }

                // read secondary stats
                reader.NextResult();
                while(reader.Read())
                {
                    string statName = reader.GetString(0);
                    int statBase = reader.GetInt32(1);
                    int statSuperficial = reader.GetInt32(2);
                    int statAggravated = reader.GetInt32(3);
                    var statProperty = (VampireV5SecondaryStat?)typeof(VampireV5SecondaryStats)
                        .GetProperty(statName)?
                        .GetValue(this.SecondaryStats, null);
                    if(statProperty is not null)
                    {
                        statProperty.BaseValue = statBase;
                        statProperty.SuperficialDamage = statSuperficial;
                        statProperty.AggravatedDamage = statAggravated;
                    }
                }

                // read discipline data
                reader.NextResult();
                while(reader.Read())
                {
                    string disciplineName = reader.GetString(0);
                    int disciplineScore = reader.GetInt32(1);
                    Disciplines.Add(disciplineName, disciplineScore);
                }

                // read discipline power data
                reader.NextResult();
                while(reader.Read())
                {
                    string powerName = reader.GetString(0);
                    DisciplinePowers.Add(VampirePower.GetDisciplinePower(powerName));
                }
            }
        }*/
        Loaded = true;
        return true;
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
    
    public bool Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Load(conn, charId);
            }
        }
    }

    public bool Load(IDbConnection conn, int charId)
    {
        return true;
    }

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Save(conn, charId);
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

    public bool Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Load(conn, charId);
            }
        }
    }

    public bool Load(IDbConnection conn, int charId)
    {
        return true;
    }

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Save(conn, charId);
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

    public bool Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Load(conn, charId);
            }
        }
    }

    public bool Load(IDbConnection conn, int charId)
    {
        return true;
    }

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                return Save(conn, charId);
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
}