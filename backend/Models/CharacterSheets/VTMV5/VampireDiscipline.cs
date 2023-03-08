using System.Collections.Generic;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

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

public class VampirePowerAmalgam
{
    public int? Level { get; set; }
    public string School { get; set; } = "";
}