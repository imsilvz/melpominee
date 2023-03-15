using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class BloodPotencyResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public BloodPotency? BloodPotency { get; set; }
}

public class BloodPotencyListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Dictionary<int, BloodPotency>? BloodPotencies { get; set; }
}

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

public class ResonanceResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Resonance? Resonance { get; set; }
}

public class ResonanceListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<Resonance>? Resonances { get; set; }
}