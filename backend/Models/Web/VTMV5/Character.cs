using Dapper;
using System.Data;
using System.Text.Json.Serialization;
using Melpominee.app.Services.Database;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class CharacterUpdateWrapper<T>
{
    public string? UpdateId { get; set; }
    public T? UpdateData { get; set; }
}

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
    public async Task Apply(VampireV5Character character)
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
                    await conn.ExecuteAsync(sql, header, transaction: trans);
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

public class VampireAttributesResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Attributes? Attributes { get; set; }
}

public class VampireAttributesUpdate
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
    public async Task Apply(VampireV5Character character)
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
        var attrProps = typeof(VampireAttributesUpdate).GetProperties();
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
                    character.Attributes = VampireV5Attributes.Load(conn, trans, (int)character.Id);
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

public class VampireSkillsResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Skills? Skills { get; set; }
}

public class VampireSkillsUpdate
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

    public async Task Apply(VampireV5Character character)
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
        var skillProps = typeof(VampireSkillsUpdate).GetProperties();
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
    }
}

public class VampireStatResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5SecondaryStats? Stats { get; set; }
}

public class VampireStatUpdate
{
    [JsonIgnore]
    public int? CharId { get; set; }
    [JsonIgnore]
    public string? StatName { get; set; }
    public int? BaseValue { get; set; }
    public int? SuperficialDamage { get; set; }
    public int? AggravatedDamage { get; set; }

    public async Task Apply(IDbConnection conn, IDbTransaction trans, VampireV5Character character, string statName)
    {
        // Id is required!
        CharId = character.Id;
        StatName = statName;
        if (CharId is null)
        {
            throw new ArgumentNullException
            (
                "VampireStatUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        // grab object thru reflection
        VampireV5SecondaryStat statObject = (VampireV5SecondaryStat)typeof(VampireV5SecondaryStats)
            .GetProperty(statName)!
            .GetValue(character.SecondaryStats)!;

        // make changes
        List<string> updList = new List<string>();
        if (BaseValue is not null)
        {
            updList.Add("BaseValue");
            statObject.BaseValue = (int)BaseValue;
        }
        if (SuperficialDamage is not null)
        {
            updList.Add("SuperficialDamage");
            statObject.SuperficialDamage = (int)SuperficialDamage;
        }
        if (AggravatedDamage is not null)
        {
            updList.Add("AggravatedDamage");
            statObject.AggravatedDamage = (int)AggravatedDamage;
        }

        var sql =
        @$"
            UPDATE melpominee_character_secondary
            SET
                {String.Join(", ", updList.Select(i => $"{i} = @{i}"))}
            WHERE charid = @CharId AND statname = @StatName;
        ";
        await conn.ExecuteAsync(sql, this, transaction: trans);
    }
}

public class VampireStatsUpdate
{
    public VampireStatUpdate? Health { get; set; }
    public VampireStatUpdate? Willpower { get; set; }
    public VampireStatUpdate? Humanity { get; set; }
    public async Task Apply(VampireV5Character character)
    {
        // Id is required!
        if (character.Id is null)
        {
            throw new ArgumentNullException
            (
                "VampireStatsUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    if (Health is not null)
                        await Health.Apply(conn, trans, character, "Health");
                    if (Willpower is not null)
                        await Willpower.Apply(conn, trans, character, "Willpower");
                    if (Humanity is not null)
                        await Humanity.Apply(conn, trans, character, "Humanity");
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

public class VampireDisciplinesResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Disciplines? Disciplines { get; set; }
}

public class VampireDisciplinesUpdate
{
    // JsonPropertyName to enforce case
    // case is important for this particular API
    // as these properties might be used as keys!
    [JsonPropertyName("Animalism")]
    public int? Animalism { get; set; }
    [JsonPropertyName("Auspex")]
    public int? Auspex { get; set; }
    [JsonPropertyName("BloodSorcery")]
    public int? BloodSorcery { get; set; }
    [JsonPropertyName("Celerity")]
    public int? Celerity { get; set; }
    [JsonPropertyName("Dominate")]
    public int? Dominate { get; set; }
    [JsonPropertyName("Fortitude")]
    public int? Fortitude { get; set; }
    [JsonPropertyName("Obfuscate")]
    public int? Obfuscate { get; set; }
    [JsonPropertyName("Oblivion")]
    public int? Oblivion { get; set; }
    [JsonPropertyName("Potence")]
    public int? Potence { get; set; }
    [JsonPropertyName("Presence")]
    public int? Presence { get; set; }
    [JsonPropertyName("Protean")]
    public int? Protean { get; set; }
    [JsonPropertyName("ThinBloodAlchemy")]
    public int? ThinBloodAlchemy { get; set; }
    public async Task Apply(VampireV5Character character)
    {
        // Id is required!
        if (character.Id is null)
        {
            throw new ArgumentNullException
            (
                "VampireSkillsUpdate.Apply called with unsaved character. A full save must be performed first."
            );
        }

        // create update listings
        var disc = character.Disciplines;
        List<object> updateList = new List<object>();
        foreach(var prop in typeof(VampireDisciplinesUpdate).GetProperties())
        {
            if(prop.Name == "CharId") { continue; }
            var Score = (int?)prop.GetValue(this);
            if(Score is not null)
            {
                if(Score > 0)
                    disc[prop.Name] = (int)Score;
                else
                    disc.Remove(prop.Name);
                updateList.Add(new { CharId = character.Id, School = prop.Name, Score = Score });
            }
        }

        // handle query
        if (updateList.Count > 0)
        {
            using (var conn = DataContext.Instance.Connect())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sql =
                        @"
                            INSERT INTO melpominee_character_disciplines
                                (charid, discipline, score)
                            VALUES
                                (@CharId, @School, @Score)
                            ON CONFLICT(charid, discipline) DO UPDATE 
                            SET
                                score = @Score;
                            DELETE FROM melpominee_character_disciplines
                            WHERE charid = @CharId 
                                AND discipline = @School 
                                AND score <= 0;
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
    public List<VampirePowerUpdateItem>? PowerIds { get; set; }
    public async Task Apply(VampireV5Character character)
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
                        character.DisciplinePowers = VampireV5DisciplinePowers.Load(conn, trans, (int)character.Id);
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

    public async Task Apply(VampireV5Character character)
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
        {
            character.Beliefs.Tenets = Tenets;
            updateList.Add("Tenets");
        }
        if (Convictions is not null)
        {
            character.Beliefs.Convictions = Convictions;
            updateList.Add("Convictions");
        }
        if (Touchstones is not null)
        {
            character.Beliefs.Touchstones = Touchstones;
            updateList.Add("Touchstones");
        }

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
                    await conn.ExecuteAsync(sql, this, transaction: trans);
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

public class VampireBackgroundMeritFlawResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5BackgroundMeritFlaw? Backgrounds { get; set; }
    public VampireV5BackgroundMeritFlaw? Merits { get; set; }
    public VampireV5BackgroundMeritFlaw? Flaws { get; set; }
}

public class VampireBackgroundMeritFlawUpdate
{
    public VampireV5BackgroundMeritFlaw? Backgrounds { get; set; }
    public VampireV5BackgroundMeritFlaw? Merits { get; set; }
    public VampireV5BackgroundMeritFlaw? Flaws { get; set; }

    public async Task Apply(VampireV5Character character)
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

    public async Task Apply(VampireV5Character character)
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
                        WHERE charid = @Id;
                    ";
                    await conn.ExecuteAsync(sql, this, transaction: trans);
                    character.Profile = VampireV5Profile.Load(conn, trans, (int)this.Id);
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