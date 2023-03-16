using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;
using Melpominee.app.Utilities.Database;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Character : BaseCharacterSheet
{
    // meta
    [JsonIgnore]
    public bool Loaded = false;
    [JsonIgnore]
    public DateTime LoadedAt = DateTime.MinValue;

    // header items
    public string Name { get; set; } = "";
    public string Concept { get; set; } = "";
    public string Chronicle { get; set; } = ""; // inherit from GameId
    public string Ambition { get; set; } = "";
    public string Desire { get; set; } = "";
    public string Sire { get; set; } = "";
    public int Generation { get; set; } = 13;
    [JsonConverter(typeof(VampireClanJsonConverter))]
    public VampireClan? Clan { get; set; } = VampireClan.GetClan("");
    [JsonConverter(typeof(VampirePredatorTypeJsonConverter))]
    public VampirePredatorType? PredatorType { get; set; } = VampirePredatorType.GetPredatorType("");

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
    public int XpSpent { get; set; } = 0;
    public int XpTotal { get; set; } = 0;

    // additional related models
    public VampireV5Beliefs Beliefs { get; set; } = new VampireV5Beliefs();
    public VampireV5Backgrounds Backgrounds { get; set; } = new VampireV5Backgrounds();
    public VampireV5Merits Merits { get; set; } = new VampireV5Merits();
    public VampireV5Flaws Flaws { get; set; } = new VampireV5Flaws();
    public VampireV5Profile Profile { get; set; } = new VampireV5Profile();

    public VampireV5Character() : base() { }

    public VampireV5Header GetHeader()
    {
        var header = new VampireV5Header()
        {
            Id = Id,
            Name = Name,
            Concept = Concept,
            Chronicle = Chronicle,
            Ambition = Ambition,
            Desire = Desire,
            Sire = Sire,
            Generation = Generation,
            Clan = Clan,
            PredatorType = PredatorType,
            Hunger = Hunger,
            Resonance = Resonance,
            BloodPotency = BloodPotency,
            XpSpent = XpSpent,
            XpTotal = XpTotal,
        };
        return header;
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
                    var result = Save(conn, trans);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans)
    {
        var sql = "";
        if (Id is null)
        {
            // we must create one!
            sql =
            @"
                INSERT INTO melpominee_characters
                    (
                        Owner, Name, Concept, Chronicle, 
                        Ambition, Desire, Sire, 
                        Generation, Clan, PredatorType,
                        Hunger, Resonance, BloodPotency,
                        XpSpent, XpTotal
                    )
                VALUES
                    (
                        @Owner, @Name, @Concept, @Chronicle,
                        @Ambition, @Desire, @Sire,
                        @Generation, @Clan, @PredatorType,
                        @Hunger, @Resonance, @BloodPotency,
                        @XpSpent, @XpTotal
                    )
                RETURNING Id;
            ";
            Id = conn.ExecuteScalar<int>(sql, this, transaction: trans);
        }
        else
        {
            sql =
            @"
                INSERT INTO melpominee_characters
                    (
                        Id, Owner, Name, Concept, Chronicle, 
                        Ambition, Desire, Sire, 
                        Generation, Clan, PredatorType,
                        Hunger, Resonance, BloodPotency,
                        XpSpent, XpTotal
                    )
                VALUES
                    (
                        @Id, @Owner, @Name, @Concept, 
                        @Chronicle, @Ambition, @Desire, @Sire,
                        @Generation, @Clan, @PredatorType,
                        @Hunger, @Resonance, @BloodPotency,
                        @XpSpent, @XpTotal
                    )
                ON CONFLICT DO UPDATE 
                SET
                    Owner = @Owner,
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
                    BloodPotency = @BloodPotency,
                    XpSpent = @XpSpent,
                    XpTotal = @XpTotal
                RETURNING Id;
            ";
            conn.ExecuteScalar<int>(sql, this, transaction: trans);
        }
        Attributes.Save(conn, trans, (int)Id);
        Skills.Save(conn, trans, (int)Id);
        SecondaryStats.Save(conn, trans, (int)Id);
        Disciplines.Save(conn, trans, (int)Id);
        DisciplinePowers.Save(conn, trans, (int)Id);
        Beliefs.Save(conn, trans, (int)Id);
        Profile.Save(conn, trans, (int)Id);
        Backgrounds.Save(conn, trans, (int)Id);
        Merits.Save(conn, trans, (int)Id);
        Flaws.Save(conn, trans, (int)Id);
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
                        Id, Owner, Name, Concept, 
                        Chronicle, Ambition, Desire, Sire, 
                        Generation, Clan, PredatorType,
                        Hunger, Resonance, BloodPotency,
                        XpSpent, XpTotal
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
                var attributes = VampireV5Attributes.Load(conn, trans, id);
                var skills = VampireV5Skills.Load(conn, trans, id);
                var secondaryStats = VampireV5SecondaryStats.Load(conn, trans, id);
                var disciplines = VampireV5Disciplines.Load(conn, trans, id);
                var disciplinePowers = VampireV5DisciplinePowers.Load(conn, trans, id);
                var beliefs = VampireV5Beliefs.Load(conn, trans, id);
                var profile = VampireV5Profile.Load(conn, trans, id);
                var backgrounds = VampireV5Backgrounds.Load(conn, trans, id);
                var merits = VampireV5Merits.Load(conn, trans, id);
                var flaws = VampireV5Flaws.Load(conn, trans, id);

                // if any fail to load, keep defaults
                user.Attributes = attributes ?? user.Attributes;
                user.Skills = skills ?? user.Skills;
                user.SecondaryStats = secondaryStats ?? user.SecondaryStats;
                user.Disciplines = disciplines ?? user.Disciplines;
                user.DisciplinePowers = disciplinePowers ?? user.DisciplinePowers;
                user.Beliefs = beliefs ?? user.Beliefs;
                user.Profile = profile ?? user.Profile;
                user.Backgrounds = backgrounds ?? user.Backgrounds;
                user.Merits = merits ?? user.Merits;
                user.Flaws = flaws ?? user.Flaws;

                try
                {
                    user.Save(conn, trans);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        user.LoadedAt = DateTime.UtcNow;
        user.Loaded = true;
        return user;
    }

    public static List<VampireV5Character> GetCharactersByUser(string email)
    {
        List<VampireV5Character> charList;
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                var sql =
                @"
                    SELECT 
                        Id, Owner, Name, Concept, 
                        Chronicle, Ambition, Desire, Sire, 
                        Generation, Clan, PredatorType,
                        Hunger, Resonance, BloodPotency,
                        XpSpent, XpTotal
                    FROM melpominee_characters
                    WHERE Owner = @Email;
                ";
                charList = conn.Query<VampireV5Character>(sql, new { Email = email }).ToList();
                foreach (var character in charList)
                {
                    // fetch dependant objects
                    var attributes = VampireV5Attributes.Load(conn, trans, (int)character.Id!);
                    var skills = VampireV5Skills.Load(conn, trans, (int)character.Id!);
                    var secondaryStats = VampireV5SecondaryStats.Load(conn, trans, (int)character.Id!);
                    var disciplines = VampireV5Disciplines.Load(conn, trans, (int)character.Id!);
                    var disciplinePowers = VampireV5DisciplinePowers.Load(conn, trans, (int)character.Id!);
                    var beliefs = VampireV5Beliefs.Load(conn, trans, (int)character.Id!);
                    var profile = VampireV5Profile.Load(conn, trans, (int)character.Id!);
                    var backgrounds = VampireV5Backgrounds.Load(conn, trans, (int)character.Id!);
                    var merits = VampireV5Merits.Load(conn, trans, (int)character.Id!);
                    var flaws = VampireV5Flaws.Load(conn, trans, (int)character.Id!);

                    // if any fail to load, keep defaults
                    character.Attributes = attributes ?? character.Attributes;
                    character.Skills = skills ?? character.Skills;
                    character.SecondaryStats = secondaryStats ?? character.SecondaryStats;
                    character.Disciplines = disciplines ?? character.Disciplines;
                    character.DisciplinePowers = disciplinePowers ?? character.DisciplinePowers;
                    character.Beliefs = beliefs ?? character.Beliefs;
                    character.Profile = profile ?? character.Profile;
                    character.LoadedAt = DateTime.UtcNow;
                    character.Loaded = true;
                }
                return charList;
            }
        }
    }
}

public class VampireV5Header
{
    public int? Id { get; set; }
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
    public int Hunger { get; set; } = 0;
    public string Resonance { get; set; } = "";
    public int BloodPotency { get; set; } = 0;
    public int XpSpent { get; set; } = 0;
    public int XpTotal { get; set; } = 0;
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
                    bool res = Save(conn, trans, charId);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var attrPropList = typeof(VampireV5Attributes).GetProperties();
        for (int i = 0; i < attrPropList.Length; i++)
        {
            var attributeProperty = attrPropList[i];
            var v5Attribute = (int)attributeProperty.GetValue(this, null)!;
            rowList.Add(new { CharId = charId, Attr = attributeProperty.Name, Score = v5Attribute });
        }

        // make sql query
        var sql =
        @"
            INSERT INTO melpominee_character_attributes
                (CharId, Attribute, Score)
            VALUES
                (@CharId, @Attr, @Score)
            ON CONFLICT DO UPDATE 
            SET
                Score = @Score;
        ";
        conn.Execute(sql, rowList, transaction: trans);
        return true;
    }

    public static VampireV5Attributes Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Attributes Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Attributes attr = new VampireV5Attributes();
        var sql =
        @"
            SELECT Attribute, Score
            FROM melpominee_character_attributes
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId }, transaction: trans);
        foreach (var result in results)
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
    public VampireV5Skill Athletics { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Brawl { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Craft { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Drive { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Firearms { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Melee { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Larceny { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Stealth { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Survival { get; set; } = new VampireV5Skill { };
    public VampireV5Skill AnimalKen { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Ettiquette { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Insight { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Intimidation { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Leadership { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Performance { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Persuasion { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Streetwise { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Subterfuge { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Academics { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Awareness { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Finance { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Investigation { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Medicine { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Occult { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Politics { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Science { get; set; } = new VampireV5Skill { };
    public VampireV5Skill Technology { get; set; } = new VampireV5Skill { };

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, trans, charId);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var skillPropList = typeof(VampireV5Skills).GetProperties();
        for (int i = 0; i < skillPropList.Length; i++)
        {
            var skillProperty = skillPropList[i];
            var v5Skill = (VampireV5Skill)skillProperty.GetValue(this, null)!;
            rowList.Add(new
            {
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
            ON CONFLICT DO UPDATE 
            SET
                Speciality = @Speciality,
                Score = @Score;
        ";
        conn.Execute(sql, rowList, transaction: trans);
        return true;
    }

    public static VampireV5Skills Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Skills Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Skills skills = new VampireV5Skills();
        var sql =
        @"
            SELECT Skill, Speciality, Score
            FROM melpominee_character_skills
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId }, transaction: trans);
        foreach (var result in results)
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
                    bool res = Save(conn, trans, charId);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        var statPropList = typeof(VampireV5SecondaryStats).GetProperties();
        for (int i = 0; i < statPropList.Length; i++)
        {
            var statProperty = statPropList[i];
            var v5Stat = (VampireV5SecondaryStat)statProperty.GetValue(this, null)!;
            rowList.Add(new
            {
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
            ON CONFLICT DO UPDATE 
            SET
                BaseValue = @BaseValue,
                SuperficialDamage = @SuperficialDamage,
                AggravatedDamage = @AggravatedDamage;
        ";
        conn.Execute(sql, rowList, transaction: trans);
        return true;
    }

    public static VampireV5SecondaryStats Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5SecondaryStats Load(IDbConnection conn, IDbTransaction trans, int charId)
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
        var results = conn.Query(sql, new { CharId = charId }, transaction: trans);
        foreach (var result in results)
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

public class VampireV5Beliefs
{
    public string Tenets { get; set; } = "";
    public string Convictions { get; set; } = "";
    public string Touchstones { get; set; } = "";

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, trans, charId);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        var sql =
        @"
            INSERT INTO melpominee_character_beliefs
                (CharId, Tenets, Convictions, Touchstones)
            VALUES
                (@CharId, @Tenets, @Convictions, @Touchstones)
            ON CONFLICT DO UPDATE 
            SET
                Tenets = @Tenets,
                Convictions = @Convictions,
                Touchstones = @Touchstones
            RETURNING Id;
        ";
        var res = conn.ExecuteScalar(sql, new
        {
            CharId = charId,
            Tenets = Tenets,
            Convictions = Convictions,
            Touchstones = Touchstones
        }, transaction: trans);
        return true;
    }

    public static VampireV5Beliefs Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Beliefs Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Beliefs? beliefs;
        var sql =
        @"
            SELECT Tenets, Convictions, Touchstones
            FROM melpominee_character_beliefs
            WHERE CharId = @Id;
        ";
        beliefs = conn.QuerySingleOrDefault<VampireV5Beliefs>(sql, new { Id = charId }, transaction: trans);
        if (beliefs is null)
        {
            return new VampireV5Beliefs();
        }
        return beliefs;
    }
}

public class VampireV5Profile
{
    public int TrueAge { get; set; } = 0;
    public int ApparentAge { get; set; } = 0;
    public string DateOfBirth { get; set; } = "01/01/0001";
    public string DateOfDeath { get; set; } = "01/01/0001";
    public string Description { get; set; } = "";
    public string History { get; set; } = "";
    public string Notes { get; set; } = "";

    public bool Save(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, trans, charId);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        var sql =
        @"
            INSERT INTO melpominee_character_profile
                (
                    CharId, TrueAge, ApparentAge, DateOfBirth, 
                    DateOfDeath, Description, History, Notes
                )
            VALUES
                (
                    @CharId, @TrueAge, @ApparentAge, @DateOfBirth, 
                    @DateOfDeath, @Description, @History, @Notes
                )
            ON CONFLICT DO UPDATE 
            SET
                TrueAge = @TrueAge, 
                ApparentAge = @ApparentAge, 
                DateOfBirth = @DateOfBirth, 
                DateOfDeath = @DateOfDeath, 
                Description = @Description, 
                History = @History, 
                Notes = @Notes
            RETURNING Id;
        ";
        var res = conn.ExecuteScalar(sql, new
        {
            CharId = charId,
            TrueAge = TrueAge,
            ApparentAge = ApparentAge,
            DateOfBirth = DateOfBirth,
            DateOfDeath = DateOfDeath,
            Description = Description,
            History = History,
            Notes = Notes
        }, transaction: trans);
        return true;
    }

    public static VampireV5Profile Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Profile Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Profile? profile;
        var sql =
        @"
            SELECT 
                TrueAge, ApparentAge, DateOfBirth,
                DateOfDeath, Description,
                History, Notes
            FROM melpominee_character_profile
            WHERE CharId = @Id;
        ";
        profile = conn.QuerySingleOrDefault<VampireV5Profile>(sql, new { Id = charId }, transaction: trans);
        if (profile is null)
        {
            return new VampireV5Profile();
        }
        return profile;
    }
}

public class MeritFlawBackground
{
    [JsonIgnore]
    public int? Id { get; set; }
    [JsonIgnore]
    public int? CharId { get; set; }
    [JsonIgnore]
    public string? ItemType { get; set; }
    public int SortOrder { get; set; } = 0;
    public string Name { get; set; } = "";
    public int Score { get; set; } = 0;
}

public class VampireV5BackgroundMeritFlaw : IDictionary<int, MeritFlawBackground>
{
    public Dictionary<int, MeritFlawBackground> data = new Dictionary<int, MeritFlawBackground>();

    public MeritFlawBackground this[int key] { get => ((IDictionary<int, MeritFlawBackground>)data)[key]; set => ((IDictionary<int, MeritFlawBackground>)data)[key] = value; }

    public ICollection<int> Keys => ((IDictionary<int, MeritFlawBackground>)data).Keys;

    public ICollection<MeritFlawBackground> Values => ((IDictionary<int, MeritFlawBackground>)data).Values;

    public int Count => ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).IsReadOnly;

    public void Add(int key, MeritFlawBackground value)
    {
        ((IDictionary<int, MeritFlawBackground>)data).Add(key, value);
    }

    public void Add(KeyValuePair<int, MeritFlawBackground> item)
    {
        ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).Clear();
    }

    public bool Contains(KeyValuePair<int, MeritFlawBackground> item)
    {
        return ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).Contains(item);
    }

    public bool ContainsKey(int key)
    {
        return ((IDictionary<int, MeritFlawBackground>)data).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<int, MeritFlawBackground>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<int, MeritFlawBackground>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<int, MeritFlawBackground>>)data).GetEnumerator();
    }

    public bool Remove(int key)
    {
        return ((IDictionary<int, MeritFlawBackground>)data).Remove(key);
    }

    public bool Remove(KeyValuePair<int, MeritFlawBackground> item)
    {
        return ((ICollection<KeyValuePair<int, MeritFlawBackground>>)data).Remove(item);
    }

    public bool TryGetValue(int key, [MaybeNullWhen(false)] out MeritFlawBackground value)
    {
        return ((IDictionary<int, MeritFlawBackground>)data).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)data).GetEnumerator();
    }

    public bool Save(int charId, string itemType)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    bool res = Save(conn, trans, charId, itemType);
                    trans.Commit();
                    return res;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId, string itemType)
    {
        // set character id for save
        foreach (var item in this.Values)
        {
            item.CharId = charId;
            item.ItemType = itemType;
        }

        var sql =
        @"
            INSERT INTO melpominee_character_meritflawbackgrounds
                (CharId, ItemType, SortOrder, Name, Score)
            VALUES
                (@CharId, @ItemType, @SortOrder, @Name, @Score)
            ON CONFLICT DO UPDATE
            SET
                Name = @Name,
                Score = @Score;
        ";
        var res = conn.Execute(sql, this.Values, transaction: trans);
        return true;
    }
}

public class VampireV5Backgrounds : VampireV5BackgroundMeritFlaw
{
    public static string ItemType { get; set; } = "background";

    public bool Save(int charId)
    {
        return Save(charId, ItemType);
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        return Save(conn, trans, charId, ItemType);
    }

    public static VampireV5Backgrounds Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Backgrounds Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Backgrounds backgrounds = new VampireV5Backgrounds();
        var sql =
        @"
            SELECT SortOrder, Name, Score
            FROM melpominee_character_meritflawbackgrounds
            WHERE CharId = @CharId AND ItemType = @ItemType;
        ";
        var results = conn.Query(sql, new { CharId = charId, ItemType = ItemType }, transaction: trans);
        foreach (var result in results)
        {
            backgrounds.Add(
            (int)result.SortOrder,
            new MeritFlawBackground()
            {
                SortOrder = (int)result.SortOrder,
                Name = result.Name,
                Score = (int)result.Score,
            });
        }
        return backgrounds;
    }
}

public class VampireV5Merits : VampireV5BackgroundMeritFlaw
{
    public static string ItemType { get; set; } = "merit";

    public bool Save(int charId)
    {
        return Save(charId, ItemType);
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        return Save(conn, trans, charId, ItemType);
    }

    public static VampireV5Merits Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Merits Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Merits merits = new VampireV5Merits();
        var sql =
        @"
            SELECT SortOrder, Name, Score
            FROM melpominee_character_meritflawbackgrounds
            WHERE CharId = @CharId AND ItemType = @ItemType;
        ";
        var results = conn.Query(sql, new { CharId = charId, ItemType = ItemType }, transaction: trans);
        foreach (var result in results)
        {
            merits.Add(
            (int)result.SortOrder,
            new MeritFlawBackground()
            {
                SortOrder = (int)result.SortOrder,
                Name = result.Name,
                Score = (int)result.Score,
            });
        }
        return merits;
    }
}

public class VampireV5Flaws : VampireV5BackgroundMeritFlaw
{
    public static string ItemType { get; set; } = "flaw";

    public bool Save(int charId)
    {
        return Save(charId, ItemType);
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int charId)
    {
        return Save(conn, trans, charId, ItemType);
    }

    public static VampireV5Flaws Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    var result = Load(conn, trans, charId);
                    trans.Commit();
                    return result;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Flaws Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Flaws merits = new VampireV5Flaws();
        var sql =
        @"
            SELECT SortOrder, Name, Score
            FROM melpominee_character_meritflawbackgrounds
            WHERE CharId = @CharId AND ItemType = @ItemType;
        ";
        var results = conn.Query(sql, new { CharId = charId, ItemType = ItemType }, transaction: trans);
        foreach (var result in results)
        {
            merits.Add(
            (int)result.SortOrder,
            new MeritFlawBackground()
            {
                SortOrder = (int)result.SortOrder,
                Name = result.Name,
                Score = (int)result.Score,
            });
        }
        return merits;
    }
}