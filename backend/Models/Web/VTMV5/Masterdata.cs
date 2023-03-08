using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class ClanResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireClan? Clan { get; set; }
}

public class ClanListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<VampireClan>? Clans { get; set; }
}

public class DisciplineResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampirePower? Discipline { get; set; }
}

public class DisciplineListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, List<VampirePower>>? Disciplines { get; set; }
}

public class PredatorTypeResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampirePredatorType? PredatorType { get; set; }
}

public class PredatorTypeListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<VampirePredatorType>? PredatorTypes { get; set; }
}