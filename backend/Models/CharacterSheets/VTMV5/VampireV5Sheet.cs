using Microsoft.Data.Sqlite;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Sheet : BaseCharacterSheet
{
    // header items
    public string Name { get; set; } = "";
    public string Concept { get; set; } = "";
    public string Chronicle { get; set; } = ""; // inherit from GameId
    public string Ambition { get; set; } = "";
    public string Desire { get; set; } = "";
    public string Sire { get; set; } = "";
    public int Generation { get; set; } = 13;
    public VampireClan? Clan { get; set; }
    public VampirePredatorType? PredatorType { get; set; }

    // primary stats
    public VampireV5Attributes Attributes { get; set; } = new VampireV5Attributes();
    public VampireV5Skills Skills { get; set; } = new VampireV5Skills();
    public VampireV5SecondaryStats SecondaryStats { get; set; } = new VampireV5SecondaryStats();

    // disciplines
    public Dictionary<string, int> Disciplines { get; set; } = new Dictionary<string, int>();
    public List<VampirePower> DisciplinePowers { get; set; } = new List<VampirePower>();

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

        using(var connection = new SqliteConnection("Data Source=data/melpominee.db"))
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
        }
        return true;
    }

    public override bool Save()
    {
        using(var connection = new SqliteConnection("Data Source=data/melpominee.db"))
        using(var cmd = connection.CreateCommand())
        {
            // open connection
            connection.Open();
            
            // insert header
            if (Id is null) {
                // no id assigned, save as a new record
                cmd.CommandText =
                @"
                    BEGIN TRANSACTION;
                    INSERT INTO melpominee_characters
                        (
                            name, concept, chronicle, 
                            ambition, desire, sire, 
                            generation, clan, predator_type,
                            hunger, resonance, blood_potency
                        )
                    VALUES
                        (
                            $name, $concept, $chronicle,
                            $ambition, $desire, $sire,
                            $generation, $clan, $predator_type,
                            $hunger, $resonance, $bloodpotency
                        )
                    RETURNING id;
                ";
                cmd.Parameters.AddWithValue("$name", Name);
                cmd.Parameters.AddWithValue("$concept", Concept);
                cmd.Parameters.AddWithValue("$chronicle", Chronicle);
                cmd.Parameters.AddWithValue("$ambition", Ambition);
                cmd.Parameters.AddWithValue("$desire", Desire);
                cmd.Parameters.AddWithValue("$sire", Sire);
                cmd.Parameters.AddWithValue("$generation", Generation);
                cmd.Parameters.AddWithValue("$clan", Clan?.Id);
                cmd.Parameters.AddWithValue("$predator_type", PredatorType?.Id);
                cmd.Parameters.AddWithValue("$hunger", Hunger);
                cmd.Parameters.AddWithValue("$resonance", Resonance);
                cmd.Parameters.AddWithValue("$bloodpotency", BloodPotency);
                
                // fetch new primary key from inserted row
                int? dbId = (int?)(long?)cmd.ExecuteScalar();
                if(dbId is null) {
                    return false;
                }
                cmd.Parameters.Clear();
                Id = dbId;

                // build attribute insert
                List<string> rows = new List<string>();
                cmd.Parameters.AddWithValue("$sheetid", Id);
                var attributePropertyList = typeof(VampireV5Attributes).GetProperties();
                for(int i=0; i<attributePropertyList.Length; i++)  
                {
                    var attributeProperty = attributePropertyList[i];
                    var v5Attribute = (int)attributeProperty.GetValue(this.Attributes, null)!;
                    rows.Add($"($sheetid, ${(i*2)}, ${(i*2)+1})");
                    cmd.Parameters.AddWithValue($"${(i*2)}", attributeProperty.Name);
                    cmd.Parameters.AddWithValue($"${(i*2)+1}", v5Attribute);
                }  
                cmd.CommandText = 
                @$"
                    INSERT INTO melpominee_character_attributes
                        (sheet_id, attribute, score)
                    VALUES
                        {String.Join(",\n", rows)};
                ";

                if(cmd.ExecuteNonQuery() <= 0) 
                {
                    return false;
                }
                cmd.Parameters.Clear();
                rows.Clear();

                // build skill insert
                cmd.Parameters.AddWithValue("$sheetid", Id);
                var skillPropertyList = typeof(VampireV5Skills).GetProperties();
                for(int i=0; i<skillPropertyList.Length; i++)  
                {
                    var skillProperty = skillPropertyList[i];
                    var v5Skill = (VampireV5Skill)skillProperty.GetValue(this.Skills, null)!;
                    rows.Add($"($sheetid, ${(i*3)}, ${(i*3)+1}, ${(i*3)+2})");
                    cmd.Parameters.AddWithValue($"${(i*3)}", skillProperty.Name);
                    cmd.Parameters.AddWithValue($"${(i*3)+1}", v5Skill.Speciality);
                    cmd.Parameters.AddWithValue($"${(i*3)+2}", v5Skill.Score);
                }  
                cmd.CommandText = 
                @$"
                    INSERT INTO melpominee_character_skills
                        (sheet_id, skill, speciality, score)
                    VALUES
                        {String.Join(",\n", rows)};
                ";

                if(cmd.ExecuteNonQuery() <= 0) 
                {
                    return false;
                }
                cmd.Parameters.Clear();
                rows.Clear();

                // build secondary insert
                cmd.Parameters.AddWithValue("$sheetid", Id);
                var statPropertyList = typeof(VampireV5SecondaryStats).GetProperties();
                for(int i=0; i<statPropertyList.Length; i++)
                {
                    var statProperty = statPropertyList[i];
                    var v5Stat = (VampireV5SecondaryStat)statProperty.GetValue(this.SecondaryStats, null)!;
                    rows.Add($"($sheetid, ${(i*4)}, ${(i*4)+1}, ${(i*4)+2}, ${(i*4)+3})");
                    cmd.Parameters.AddWithValue($"${(i*4)}", statProperty.Name);
                    cmd.Parameters.AddWithValue($"${(i*4)+1}", v5Stat.BaseValue);
                    cmd.Parameters.AddWithValue($"${(i*4)+2}", v5Stat.SuperficialDamage);
                    cmd.Parameters.AddWithValue($"${(i*4)+3}", v5Stat.AggravatedDamage);
                }
                cmd.CommandText =
                @$"
                    INSERT INTO melpominee_character_secondary
                        (sheet_id, stat_name, base_value, superficial_damage, aggravated_damage)
                    VALUES
                        {String.Join(",\n", rows)};
                ";

                if(cmd.ExecuteNonQuery() <= 0) 
                {
                    return false;
                }
                cmd.Parameters.Clear();
                rows.Clear();

                // build discipline insert
                int counter = 0;
                cmd.Parameters.AddWithValue("$sheetid", Id);
                foreach(var discipline in Disciplines)
                {
                    rows.Add($"($sheetid, ${2*counter}, ${1+2*counter})");
                    cmd.Parameters.AddWithValue($"${2*counter}", discipline.Key);
                    cmd.Parameters.AddWithValue($"${1+2*counter}", discipline.Value);
                    counter++;
                }
                cmd.CommandText =
                @$"
                    INSERT INTO melpominee_character_disciplines
                        (sheet_id, discipline, score)
                    VALUES
                        {String.Join(",\n", rows)};
                ";
                if(cmd.ExecuteNonQuery() < Disciplines.Count) 
                {
                    return false;
                }
                cmd.Parameters.Clear();
                rows.Clear();

                // build discipline power insert
                cmd.Parameters.AddWithValue("$sheetid", Id);
                for(int i=0; i<DisciplinePowers.Count; i++)
                {
                    rows.Add($"($sheetid, ${i})");
                    cmd.Parameters.AddWithValue($"${i}", DisciplinePowers[i].Id);
                }
                cmd.CommandText =
                @$"
                    INSERT INTO melpominee_character_discipline_powers
                        (sheet_id, power_name)
                    VALUES
                        {String.Join(",\n", rows)};
                ";
                if(cmd.ExecuteNonQuery() < DisciplinePowers.Count) 
                {
                    return false;
                }
                cmd.Parameters.Clear();
                rows.Clear();

                cmd.CommandText = "COMMIT TRANSACTION";
                cmd.ExecuteNonQuery();
            } else {
                // id is assigned, update existing record
                throw new NotImplementedException();
            }

            // close connection
            connection.Close();
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
}