using Microsoft.AspNetCore.Mvc;
using Melpominee.app.Models.Web.VTMV5;
using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Controllers;

[ApiController]
[Route("vtmv5/[controller]/[action]")]
public class MasterdataController : ControllerBase
{
    private readonly ILogger<MasterdataController> _logger;
    public MasterdataController(ILogger<MasterdataController> logger)
    {
        _logger = logger;
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
        List<VampireClan> clanList = new List<VampireClan>();
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                if (type.BaseType == typeof(VampireClan))
                {
                    VampireClan reflectClan = (VampireClan)Activator.CreateInstance(type)!;
                    clanList.Add(reflectClan);
                }
            }
        }
        return new ClanListResponse
        {
            Success = true,
            Clans = clanList
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
        foreach(var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in asm.GetTypes())
            {
                if (type.BaseType == typeof(VampirePower))
                {
                    List<VampirePower>? powerList;
                    VampirePower reflectDisc = (VampirePower)Activator.CreateInstance(type)!;
                    string schoolName = reflectDisc.School.Replace(" ", "").Replace("-", "");
                    if(!disciplineList.TryGetValue(schoolName, out powerList))
                    {
                        powerList = new List<VampirePower>();
                        disciplineList.Add(schoolName, new List<VampirePower>());
                    }
                    powerList.Add(reflectDisc);
                }
            }
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
}