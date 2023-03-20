using Dapper;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melpominee.app.Services.Database;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampirePowerAmalgam
{
    public int? Level { get; set; }
    public string School { get; set; } = "";
}

public class VampirePower
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string School { get; set; } = "";
    public int Level { get; set; } = 0;
    public string? Prerequisite { get; set; } = null;
    public VampirePowerAmalgam? Amalgam { get; set; } = null;
    public string Cost { get; set; } = "";
    public string Duration { get; set; } = "";
    public string DicePool { get; set; } = "";
    public string OpposingPool { get; set; } = "";
    public string Effect { get; set; } = "";
    public string? AdditionalNotes { get; set; } = null;
    public string Source { get; set; } = "";

    public static Dictionary<string, VampirePower> PowerDict = new Dictionary<string, VampirePower>();
    static VampirePower()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Melpominee.app.backend.Data.DisciplinePowers.json";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is not null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var powerDict = JsonSerializer.Deserialize<Dictionary<string, VampirePower>>(json);
                if (powerDict is not null)
                {
                    PowerDict = powerDict;
                }
            }
        }
    }

    public static VampirePower GetDisciplinePower(string id)
    {
        VampirePower? disc = null;
        if (PowerDict.TryGetValue(id, out disc))
        {
            return disc;
        }
        throw new ArgumentException($"'{id}' is not a valid VampirePower identifier.");
    }
}

public class VampireV5Disciplines : IDictionary<string, int>, ICharacterSaveable
{
    public int this[string key] { get => ((IDictionary<string, int>)_data)[key]; set => ((IDictionary<string, int>)_data)[key] = value; }

    public ICollection<string> Keys => ((IDictionary<string, int>)_data).Keys;

    public ICollection<int> Values => ((IDictionary<string, int>)_data).Values;

    public int Count => ((ICollection<KeyValuePair<string, int>>)_data).Count;

    public bool IsReadOnly => ((ICollection<KeyValuePair<string, int>>)_data).IsReadOnly;

    private Dictionary<string, int> _data { get; } = new Dictionary<string, int>();

    public void Add(string key, int value)
    {
        ((IDictionary<string, int>)_data).Add(key, value);
    }

    public void Add(KeyValuePair<string, int> item)
    {
        ((ICollection<KeyValuePair<string, int>>)_data).Add(item);
    }

    public void Clear()
    {
        ((ICollection<KeyValuePair<string, int>>)_data).Clear();
    }

    public bool Contains(KeyValuePair<string, int> item)
    {
        return ((ICollection<KeyValuePair<string, int>>)_data).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, int>)_data).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, int>>)_data).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, int>>)_data).GetEnumerator();
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, int>)_data).Remove(key);
    }

    public bool Remove(KeyValuePair<string, int> item)
    {
        return ((ICollection<KeyValuePair<string, int>>)_data).Remove(item);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out int value)
    {
        return ((IDictionary<string, int>)_data).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }

    public bool Save(int? charId)
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
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int? charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        foreach(var discipline in this)
        {
            rowList.Add(new { 
                CharId = charId,
                Discipline =  discipline.Key,
                Score = discipline.Value,
            });
        }

        // delete old values to handle removal
        var sql =
        @"
            DELETE FROM 
            melpominee_character_disciplines
            WHERE charid = @CharId;
        ";
        conn.Execute(sql, new { CharId = charId }, transaction: trans);

        // make sql query
        sql =
        @"
            INSERT INTO melpominee_character_disciplines
                (charid, discipline, score)
            VALUES
                (@CharId, @Discipline, @Score)
            ON CONFLICT(charid, discipline) DO UPDATE 
            SET
                score = @Score;
        ";
        conn.Execute(sql, rowList, transaction: trans);
        return true;
    }

    public static VampireV5Disciplines Load(int charId)
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
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5Disciplines Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5Disciplines disciplines = new VampireV5Disciplines();
        var sql = 
        @"
            SELECT discipline, score
            FROM melpominee_character_disciplines
            WHERE charid = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId }, transaction: trans);
        foreach(var result in results)
        {
            disciplines[(string)result.discipline] = (int)result.score;
        }
        return disciplines;
    }
}

public class VampireV5DisciplinePowers : IList<VampirePower>, ICharacterSaveable
{
    public VampirePower this[int index] { get => ((IList<VampirePower>)_data)[index]; set => ((IList<VampirePower>)_data)[index] = value; }

    public int Count => ((ICollection<VampirePower>)_data).Count;

    public bool IsReadOnly => ((ICollection<VampirePower>)_data).IsReadOnly;

    private List<VampirePower> _data { get; } = new List<VampirePower>();

    public void Add(VampirePower item)
    {
        ((ICollection<VampirePower>)_data).Add(item);
    }

    public void Clear()
    {
        ((ICollection<VampirePower>)_data).Clear();
    }

    public bool Contains(VampirePower item)
    {
        return ((ICollection<VampirePower>)_data).Contains(item);
    }

    public void CopyTo(VampirePower[] array, int arrayIndex)
    {
        ((ICollection<VampirePower>)_data).CopyTo(array, arrayIndex);
    }

    public IEnumerator<VampirePower> GetEnumerator()
    {
        return ((IEnumerable<VampirePower>)_data).GetEnumerator();
    }

    public int IndexOf(VampirePower item)
    {
        return ((IList<VampirePower>)_data).IndexOf(item);
    }

    public void Insert(int index, VampirePower item)
    {
        ((IList<VampirePower>)_data).Insert(index, item);
    }

    public bool Remove(VampirePower item)
    {
        return ((ICollection<VampirePower>)_data).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<VampirePower>)_data).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }

    public bool Save(int? charId)
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
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public List<string> GetIdList()
    {
        var ret = new List<string>();
        foreach(var power in this)
        {
            ret.Add(power.Id);
        }
        return ret;
    }

    public bool Save(IDbConnection conn, IDbTransaction trans, int? charId)
    {
        // gather values
        List<object> rowList = new List<object>();
        for(int i=0; i<this.Count; i++)
        {
            rowList.Add(new { 
                CharId = charId,
                PowerId =  this[i].Id,
            });
        }

        // delete old values
        var sql =
        @"
            DELETE FROM 
            melpominee_character_discipline_powers
            WHERE charid = @CharId;
        ";
        conn.Execute(sql, new { CharId = charId }, transaction: trans);

        // make sql query
        sql =
        @"
            INSERT INTO melpominee_character_discipline_powers
                (charid, powerid)
            VALUES
                (@CharId, @PowerId);
        ";
        conn.Execute(sql, rowList, transaction: trans);
        return true;
    }

    public static VampireV5DisciplinePowers Load(int charId)
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
                catch(Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }

    public static VampireV5DisciplinePowers Load(IDbConnection conn, IDbTransaction trans, int charId)
    {
        VampireV5DisciplinePowers powers = new VampireV5DisciplinePowers();
        var sql = 
        @"
            SELECT powerid
            FROM melpominee_character_discipline_powers
            WHERE charid = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId }, transaction: trans);
        foreach(var result in results)
        {
            var power = VampirePower.GetDisciplinePower(result.powerid);
            powers.Add(power);
        }
        return powers;
    }
}

public class VampirePowerListJsonConverter : JsonConverter<VampireV5DisciplinePowers>
{
    public override VampireV5DisciplinePowers? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var powerList = JsonSerializer.Deserialize<List<string>>(ref reader, options);
        VampireV5DisciplinePowers powers = new VampireV5DisciplinePowers();

        // default
        if (powerList is null)
            return powers;

        // iterate and get
        foreach(var powerId in powerList)
        {
            powers.Add(VampirePower.GetDisciplinePower(powerId));
        }
        return powers;
    }

    public override void Write(Utf8JsonWriter writer, VampireV5DisciplinePowers value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach(var item in value)
        {
            writer.WriteStringValue(item.Id);
        }
        writer.WriteEndArray();
    }
}