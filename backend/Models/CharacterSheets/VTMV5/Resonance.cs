using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;
public class Resonance
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Emotions { get; set; } = "";
    public List<string> Disciplines { get; set; } = new List<string>();

    public static Dictionary<string, Resonance> ResonanceDict = new Dictionary<string, Resonance>();
    static Resonance()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Melpominee.app.backend.Config.Resonance.json";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is not null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var resonanceDict = JsonSerializer.Deserialize<Dictionary<string, Resonance>>(json);
                if (resonanceDict is not null)
                {
                    ResonanceDict = resonanceDict;
                }
            }
        }
    }

    public static Resonance GetResonance(string resonance)
    {
        Resonance? ret;
        if (ResonanceDict.TryGetValue(resonance, out ret))
            return ret;
        return new Resonance();
    }
}