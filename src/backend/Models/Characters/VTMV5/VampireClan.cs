using Dapper;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampireClan
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Compulsion { get; set; } = "";
    public string Bane { get; set; } = "";
    public List<string> Disciplines { get; set; } = new List<string>();

    public static Dictionary<string, VampireClan> ClanDict = new Dictionary<string, VampireClan>();
    static VampireClan()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Melpominee.app.backend.Data.Clans.json";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is not null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var clanDict = JsonSerializer.Deserialize<Dictionary<string, VampireClan>>(json);
                if (clanDict is not null)
                {
                    ClanDict = clanDict;
                }
            }
        }
    }

    public static VampireClan GetClan(string name)
    {
        VampireClan? clan = null;
        if (ClanDict.TryGetValue(name, out clan))
        {
            return clan;
        }
        throw new ArgumentException($"'{name}' is not a valid VampireClan identifier.");
    }
}

public class VampireClanJsonConverter : JsonConverter<VampireClan>
{
    public override VampireClan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? clanId = reader.GetString();
        if (clanId is not null)
        {
            return VampireClan.GetClan(clanId);
        }
        return VampireClan.GetClan("");
    }

    public override void Write(Utf8JsonWriter writer, VampireClan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}

public class VampireClanTypeHandler : SqlMapper.TypeHandler<VampireClan>
{
    public override VampireClan Parse(object value)
    {
        return VampireClan.GetClan((string) value);
    }

    public override void SetValue(IDbDataParameter parameter, VampireClan value)
    {
        parameter.Value = value.Id;
    }
}