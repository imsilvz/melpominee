using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Controllers;

[Authorize]
[ApiController]
[Route("vtmv5/[controller]/[action]")]
public class MasterdataController : ControllerBase
{
    private readonly ILogger<MasterdataController> _logger;
    public MasterdataController(ILogger<MasterdataController> logger)
    {
        _logger = logger;
    }

    [ActionName("bloodpotency")]
    [HttpGet(Name = "Get Blood Potency")]
    public BloodPotencyResponse GetBloodPotency([FromQuery] int id)
    {
        return new BloodPotencyResponse()
        {
            Success = BloodPotency.BloodPotencyDict.ContainsKey(id),
            BloodPotency = BloodPotency.GetPotency(id)
        };
    }

    [ActionName("bloodpotency/list")]
    [HttpGet(Name = "Get Blood Potency List")]
    public BloodPotencyListResponse GetBloodPotencyList()
    {
        return new BloodPotencyListResponse()
        {
            Success = true,
            BloodPotencies = BloodPotency.BloodPotencyDict
        };
    }

    [ActionName("clan")]
    [HttpGet(Name = "Get Clan")]
    public ClanResponse GetClan([FromQuery] string id)
    {
        VampireClan clan;
        try
        {
            clan = VampireClan.GetClan(id);
        }
        catch(ArgumentException)
        {
            return new ClanResponse
            {
                Success = false,
                Error = "invalid_clan"
            };
        }
        return new ClanResponse
        {
            Success = true,
            Clan = clan
        };
    }

    [ActionName("clan/list")]
    [HttpGet(Name = "Get Clan List")]
    public ClanListResponse GetClanList()
    {
        return new ClanListResponse
        {
            Success = true,
            Clans = VampireClan.ClanDict.Values.ToList()
        };
    }

    [ActionName("discipline")]
    [HttpGet(Name = "Get Discipline")]
    public DisciplineResponse GetDisciplinePower([FromQuery] string id)
    {
        VampirePower power;
        try
        {
            power = VampirePower.GetDisciplinePower(id);
        }
        catch(ArgumentException)
        {
            return new DisciplineResponse
            {
                Success = false,
                Error = "invalid_power"
            };
        }
        return new DisciplineResponse
        {
            Success = true,
            Discipline = power,
        };
    }

    [ActionName("discipline/list")]
    [HttpGet(Name = "Get Discipline List")]
    public DisciplineListResponse GetDisciplinePowerList()
    {
        Dictionary<string, List<VampirePower>> disciplineList = new Dictionary<string, List<VampirePower>>();
        foreach(var power in VampirePower.PowerDict)
        {
            List<VampirePower>? powerList;
            string schoolName = power.Value.School.Replace(" ", "").Replace("-", "");
            if(!disciplineList.TryGetValue(schoolName, out powerList))
            {
                powerList = new List<VampirePower>();
                disciplineList.Add(schoolName, powerList);
            }
            powerList.Add(power.Value);
        }
        return new DisciplineListResponse
        {
            Success = true,
            Disciplines = disciplineList
        };
    }

    [ActionName("predatortype")]
    [HttpGet(Name = "Get Predator Type")]
    public PredatorTypeResponse GetPredatorType(string id)
    {
        VampirePredatorType predatorType;
        try
        {
            predatorType = VampirePredatorType.GetPredatorType(id);
        }
        catch(ArgumentException)
        {
            return new PredatorTypeResponse
            {
                Success = false,
                Error = "invalid_predatortype"
            };
        }
        return new PredatorTypeResponse
        {
            Success = true,
            PredatorType = predatorType
        };
    }

    [ActionName("predatortype/list")]
    [HttpGet(Name = "Get Predator Type List")]
    public PredatorTypeListResponse GetPredatorTypeList()
    {
        List<VampirePredatorType> predatorList = new List<VampirePredatorType>();
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                if (type.BaseType == typeof(VampirePredatorType))
                {
                    VampirePredatorType reflectType = (VampirePredatorType)Activator.CreateInstance(type)!;
                    predatorList.Add(reflectType);
                }
            }
        }
        return new PredatorTypeListResponse
        {
            Success = true,
            PredatorTypes = predatorList
        };
    }

    [ActionName("resonance")]
    [HttpGet(Name = "Get Resonance")]
    public ResonanceResponse GetResonance(string id)
    {
        Resonance resonance;
        try
        {
            resonance = Resonance.GetResonance(id);
        }
        catch(ArgumentException)
        {
            return new ResonanceResponse
            {
                Success = false,
                Error = "invalid_resonance"
            };
        }
        return new ResonanceResponse
        {
            Success = true,
            Resonance = resonance
        };
    }
    
    [ActionName("resonance/list")]
    [HttpGet(Name = "Get Resonance List")]
    public ResonanceListResponse GetResonanceList()
    {
        return new ResonanceListResponse
        {
            Success = true,
            Resonances = Resonance.ResonanceDict.Values.ToList()
        };
    }
}