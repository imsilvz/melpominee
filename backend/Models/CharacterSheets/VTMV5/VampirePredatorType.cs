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