using System.Text.Json;
using System.Text.Json.Serialization;
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

public class VampirePowerListJsonConverter : JsonConverter<List<VampirePower>>
{
    public override List<VampirePower>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, List<VampirePower> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach(var item in value)
        {
            writer.WriteStringValue(item.Id);
        }
        writer.WriteEndArray();
    }
}