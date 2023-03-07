using System.Collections.Generic;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public abstract class VampireDiscipline
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string School { get; }
    public abstract int Level { get; }
    public virtual string? Prerequisite { get; } = null;
    public virtual VampireDisciplineAmalgam? Amalgam { get; } = null;
    public abstract string Cost { get; }
    public abstract string Duration { get; }
    public abstract string DicePool { get; }
    public abstract string OpposingPool { get; }
    public abstract string Effect { get; }
    public virtual string? AdditionalNotes { get; } = null;
    public abstract string Source { get; }

    private static Dictionary<string, VampireDiscipline>? DisciplineDict;
    public static VampireDiscipline GetDiscipline(string id)
    {
        VampireDiscipline? disc = null;
        if (DisciplineDict is null)
        {
            // first run
            DisciplineDict = new Dictionary<string, VampireDiscipline>();
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == typeof(VampireDiscipline))
                    {
                        VampireDiscipline reflectDisc = (VampireDiscipline)Activator.CreateInstance(type)!;
                        DisciplineDict.Add(reflectDisc.Id, reflectDisc);
                    }
                }
            }
        }
        if (DisciplineDict.TryGetValue(id, out disc))
        {
            return disc;
        }
        throw new ArgumentException($"'{id}' is not a valid VampireDiscipline identifier.");
    }
}

public class VampireDisciplineAmalgam
{
    public int? Level { get; set; }
    public string School { get; set; } = "";
}