using Dapper;
using System.Data;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.Characters.VTMV5;

public abstract class VampirePredatorType
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string RollInfo { get; }
    public abstract List<string> EffectList { get; }

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
                var predDict = JsonSerializer.Deserialize<List<VampirePredatorType>>(json);
                if (predDict is not null)
                {
                    foreach(var predType in predDict)
                    {
                        TypeDict.Add(predType.Id, predType);
                    }
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