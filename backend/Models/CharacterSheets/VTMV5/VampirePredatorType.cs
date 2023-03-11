using Dapper;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public abstract class VampirePredatorType
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string RollInfo { get; }
    public abstract List<string> EffectList { get; }
    private static Dictionary<string, VampirePredatorType>? TypeDict;
    public static VampirePredatorType GetPredatorType(string id)
    {
        VampirePredatorType? predatorType = null;
        if (TypeDict is null)
        {
            // first run
            TypeDict = new Dictionary<string, VampirePredatorType>();
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == typeof(VampirePredatorType))
                    {
                        VampirePredatorType reflectType = (VampirePredatorType)Activator.CreateInstance(type)!;
                        TypeDict.Add(reflectType.Id, reflectType);
                    }
                }
            }
        }
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
        throw new NotImplementedException();
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