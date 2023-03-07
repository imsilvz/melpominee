using System.Collections.Generic;
namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public abstract class VampireClan
{
    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract string Bane { get; }
    public abstract List<string> Disciplines { get; }

    private static Dictionary<string, VampireClan>? ClanDict;
    public static VampireClan GetClan(string name)
    {
        VampireClan? clan = null;
        if (ClanDict is null)
        {
            // first run
            ClanDict = new Dictionary<string, VampireClan>();
            foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in asm.GetTypes())
                {
                    if (type.BaseType == typeof(VampireClan))
                    {
                        VampireClan reflectClan = (VampireClan)Activator.CreateInstance(type)!;
                        ClanDict.Add(reflectClan.Id, reflectClan);
                    }
                }
            }
        }
        if (ClanDict.TryGetValue(name, out clan))
        {
            return clan;
        }
        throw new ArgumentException($"'{name}' is not a valid VampireClan identifier");
    }
}