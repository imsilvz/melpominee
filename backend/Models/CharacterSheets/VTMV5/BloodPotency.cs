using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;
public class BloodPotency
{
    public int Potency { get; set; } = 0;
    public int BloodSurge { get; set; } = 0;
    public int DamageMend { get; set; } = 0;
    public int PowerBonus { get; set; } = 0;
    public int RouseReroll { get; set; } = 0;
    public int BaneSeverity { get; set; } = 0;
    public List<string> FeedingPenalty { get; set; } = new List<string>();

    public static Dictionary<int, BloodPotency> BloodPotencyDict = new Dictionary<int, BloodPotency>();
    static BloodPotency()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = "Melpominee.app.backend.Data.BloodPotency.json";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is not null)
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                var potencyDict = JsonSerializer.Deserialize<Dictionary<int, BloodPotency>>(json);
                if (potencyDict is not null)
                {
                    foreach(var item in potencyDict)
                    {
                        item.Value.Potency = item.Key;
                    };
                    BloodPotencyDict = potencyDict;
                }
            }
        }
    }

    public static BloodPotency GetPotency(int potency)
    {
        BloodPotency? ret;
        if (BloodPotencyDict.TryGetValue(potency, out ret))
            return ret;
        return new BloodPotency();
    }
}