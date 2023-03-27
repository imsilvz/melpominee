using Dapper;
using System.Data;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.Characters.VTMV5;

public class VampirePredatorType
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string RollInfo { get; set; } = "";
    public List<string> EffectList { get; set; } = new List<string>();

    public static Dictionary<string, VampirePredatorType> TypeDict = new Dictionary<string, VampirePredatorType>();
    static VampirePredatorType()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Melpominee.app.backend.Data.PredatorTypes.json";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is not null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                JsonSerializerOptions jsonOptions = new()
                {
                    PropertyNameCaseInsensitive = true,
                };
                var predDict = JsonSerializer.Deserialize<Dictionary<string, VampirePredatorType>>(json, jsonOptions);
                if (predDict is not null)
                {
                    TypeDict = predDict;
                }
            }
        }
    }

    public static VampirePredatorType GetPredatorType(string id)
    {
        VampirePredatorType? predatorType = null;
        if (TypeDict.TryGetValue(id, out predatorType))
        {
            return predatorType;
        }
        throw new ArgumentException($"'{id}' is not a valid VampirePredatorType identifier.");
    }
}

public class VampirePredatorTypeJsonConverter : JsonConverter<VampirePredatorType>
{
    public override VampirePredatorType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? predId = reader.GetString();
        if (predId is not null)
        {
            return VampirePredatorType.GetPredatorType(predId);
        }
        return VampirePredatorType.GetPredatorType("");
    }

    public override void Write(Utf8JsonWriter writer, VampirePredatorType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}

public class VampirePredatorTypeHandler : SqlMapper.TypeHandler<VampirePredatorType>
{
    public override VampirePredatorType Parse(object value)
    {
        return VampirePredatorType.GetPredatorType((string) value);
    }

    public override void SetValue(IDbDataParameter parameter, VampirePredatorType value)
    {
        parameter.Value = value.Id;
    }
}