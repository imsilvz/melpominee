using Dapper;
using System.Data;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Melpominee.app.Utilities.Database;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampirePowerAmalgam
{
    public int? Level { get; set; }
    public string School { get; set; } = "";
}

public abstract class VampirePower
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string School { get; }
    public abstract int Level { get; }
    public virtual string? Prerequisite { get; } = null;
    public virtual VampirePowerAmalgam? Amalgam { get; } = null;
    public abstract string Cost { get; }
    public abstract string Duration { get; }
    public abstract string DicePool { get; }
    public abstract string OpposingPool { get; }
    public abstract string Effect { get; }
    public virtual string? AdditionalNotes { get; } = null;
    public abstract string Source { get; }

    private static Dictionary<string, VampirePower>? PowerDict;
    public static VampirePower GetDisciplinePower(string id)
    {
        VampirePower? disc = null;
        if (PowerDict is null)
        {
            // first run
            PowerDict = new Dictionary<string, VampirePower>();
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == typeof(VampirePower))
                    {
                        VampirePower reflectDisc = (VampirePower)Activator.CreateInstance(type)!;
                        PowerDict.Add(reflectDisc.Id, reflectDisc);
                    }
                }
            }
        }
        if (PowerDict.TryGetValue(id, out disc))
        {
            return disc;
        }
        throw new ArgumentException($"'{id}' is not a valid VampirePower identifier.");
    }
}

public class VampirePowerJsonConverter : JsonConverter<VampirePower>
{
    public override VampirePower? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, VampirePower value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}

public class VampireV5Disciplines : IDictionary<string, int>
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
        List<string> idList = new List<string>();
        List<object> rowList = new List<object>();
        foreach(var discipline in this)
        {
            idList.Add(discipline.Key);
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
            WHERE CharId = @CharId
                AND Discipline IN @DisciplineIds
        ";
        conn.Execute(sql, new { CharId = charId, DisciplineIds = idList });

        // make sql query
        sql =
        @"
            INSERT INTO melpominee_character_disciplines
                (CharId, Discipline, Score)
            VALUES
                (@CharId, @Discipline, @Score)
            ON CONFLICT DO
            UPDATE SET
                Score = @Score;
        ";
        conn.Execute(sql, rowList);
        return true;
    }

    public static VampireV5Disciplines? Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            return Load(conn, charId);
        }
    }

    public static VampireV5Disciplines? Load(IDbConnection conn, int charId)
    {
        VampireV5Disciplines disciplines = new VampireV5Disciplines();
        var sql = 
        @"
            SELECT Discipline, Score
            FROM melpominee_character_disciplines
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId });
        foreach(var result in results)
        {
            disciplines[(string)result.Discipline] = (int)result.Score;
        }
        return disciplines;
    }
}

public class VampireV5DisciplinePowers : IList<VampirePower>
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
        List<string> idList = new List<string>();
        List<object> rowList = new List<object>();
        for(int i=0; i<this.Count; i++)
        {
            idList.Add(this[i].Id);
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
            WHERE CharId = @CharId
                AND PowerId IN @PowerIds
        ";
        conn.Execute(sql, new { CharId = charId, PowerIds = idList });

        // make sql query
        sql =
        @"
            INSERT INTO melpominee_character_discipline_powers
                (CharId, PowerId)
            VALUES
                (@CharId, @PowerId);
        ";
        conn.Execute(sql, rowList);
        return true;
    }

    public static VampireV5DisciplinePowers? Load(int charId)
    {
        using (var conn = DataContext.Instance.Connect())
        {
            conn.Open();
            return Load(conn, charId);
        }
    }

    public static VampireV5DisciplinePowers? Load(IDbConnection conn, int charId)
    {
        VampireV5DisciplinePowers powers = new VampireV5DisciplinePowers();
        var sql = 
        @"
            SELECT PowerId
            FROM melpominee_character_discipline_powers
            WHERE CharId = @CharId;
        ";
        var results = conn.Query(sql, new { CharId = charId });
        foreach(var result in results)
        {
            var power = VampirePower.GetDisciplinePower(result.PowerId);
            powers.Add(power);
        }
        return powers;
    }
}

public class VampirePowerListJsonConverter : JsonConverter<VampireV5DisciplinePowers>
{
    public override VampireV5DisciplinePowers? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
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