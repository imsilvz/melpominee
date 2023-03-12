using Melpominee.app.Models.CharacterSheets.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class VampireCharacterResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Character? Character { get; set; }
}

public class VampireCharacterUpdate
{
    public string? Name { get; set; }
    public string? Concept { get; set; }
    public string? Chronicle { get; set; }
    public string? Ambition { get; set; }
    public string? Desire { get; set; }
    public string? Sire { get; set; }
    public int? Generation { get; set; }
    public string? Clan { get; set; }
    public string? PredatorType { get; set; }
    public void Apply(VampireV5Character character)
    {
        character.Name = Name ?? character.Name;
        character.Concept = Concept ?? character.Concept;
        character.Chronicle = Chronicle ?? character.Chronicle;
        character.Ambition = Ambition ?? character.Ambition;
        character.Desire = Desire ?? character.Desire;
        character.Sire = Sire ?? character.Sire;
        character.Generation = Generation ?? character.Generation;
        character.Clan = Clan is not null ? 
            VampireClan.GetClan(Clan) : character.Clan;
        character.PredatorType = PredatorType is not null ? 
            VampirePredatorType.GetPredatorType(PredatorType) : character.PredatorType;
        character.Save();
    }
}

public class VampireAttributesResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Attributes? Attributes { get; set; }
}

public class VampireAttributesUpdate
{
    public int? Strength { get; set; }
    public int? Dexterity { get; set; }
    public int? Stamina { get; set; }
    public int? Charisma { get; set; }
    public int? Manipulation { get; set; }
    public int? Composure { get; set; }
    public int? Intelligence { get; set; }
    public int? Wits { get; set; }
    public int? Resolve { get; set; }
    public void Apply(VampireV5Character character)
    {
        var attr = character.Attributes;
        attr.Strength = Strength ?? attr.Strength;
        attr.Dexterity = Dexterity ?? attr.Dexterity;
        attr.Stamina = Stamina ?? attr.Stamina;
        attr.Charisma = Charisma ?? attr.Charisma;
        attr.Manipulation = Manipulation ?? attr.Manipulation;
        attr.Composure = Composure ?? attr.Composure;
        attr.Intelligence = Intelligence ?? attr.Intelligence;
        attr.Wits = Wits ?? attr.Wits;
        attr.Resolve = Resolve ?? attr.Resolve;
        if (character.Id is not null)
        {
            attr.Save((int)character.Id);
        }
        else
        {
            character.Save();
        }
    }
}

public class VampireSkillsResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Skills? Skills { get; set; }
}

public class VampireSkillsUpdate
{
    public VampireV5Skill? Athletics { get; set; }
    public VampireV5Skill? Brawl { get; set; }
    public VampireV5Skill? Craft { get; set; }
    public VampireV5Skill? Drive { get; set; }
    public VampireV5Skill? Firearms { get; set; }
    public VampireV5Skill? Melee { get; set; }
    public VampireV5Skill? Larceny { get; set; }
    public VampireV5Skill? Stealth { get; set; }
    public VampireV5Skill? Survival { get; set; }
    public VampireV5Skill? AnimalKen { get; set; }
    public VampireV5Skill? Ettiquette { get; set; }
    public VampireV5Skill? Insight { get; set; }
    public VampireV5Skill? Intimidation { get; set; }
    public VampireV5Skill? Leadership { get; set; }
    public VampireV5Skill? Performance { get; set; }
    public VampireV5Skill? Persuasion { get; set; }
    public VampireV5Skill? Streetwise { get; set; }
    public VampireV5Skill? Subterfuge { get; set; }
    public VampireV5Skill? Academics { get; set; }
    public VampireV5Skill? Awareness { get; set; }
    public VampireV5Skill? Finance { get; set; }
    public VampireV5Skill? Investigation { get; set; }
    public VampireV5Skill? Medicine { get; set; }
    public VampireV5Skill? Occult { get; set; }
    public VampireV5Skill? Politics { get; set; }
    public VampireV5Skill? Science { get; set; }
    public VampireV5Skill? Technology { get; set; }

    public void Apply(VampireV5Character character)
    {
        var skills = character.Skills;
        skills.Athletics = Athletics ?? skills.Athletics;
        skills.Brawl = Brawl ?? skills.Brawl;
        skills.Craft = Craft ?? skills.Craft;
        skills.Drive = Drive ?? skills.Drive;
        skills.Firearms = Firearms ?? skills.Firearms;
        skills.Melee = Melee ?? skills.Melee;
        skills.Larceny = Larceny ?? skills.Larceny;
        skills.Stealth = Stealth ?? skills.Stealth;
        skills.Survival = Survival ?? skills.Survival;
        skills.AnimalKen = AnimalKen ?? skills.AnimalKen;
        skills.Ettiquette = Ettiquette ?? skills.Ettiquette;
        skills.Insight = Insight ?? skills.Insight;
        skills.Intimidation = Intimidation ?? skills.Intimidation;
        skills.Leadership = Leadership ?? skills.Leadership;
        skills.Performance = Performance ?? skills.Performance;
        skills.Persuasion = Persuasion ?? skills.Persuasion;
        skills.Streetwise = Streetwise ?? skills.Streetwise;
        skills.Subterfuge = Subterfuge ?? skills.Subterfuge;
        skills.Academics = Academics ?? skills.Academics;
        skills.Awareness = Awareness ?? skills.Awareness;
        skills.Finance = Finance ?? skills.Finance;
        skills.Investigation = Investigation ?? skills.Investigation;
        skills.Medicine = Medicine ?? skills.Medicine;
        skills.Occult = Occult ?? skills.Occult;
        skills.Politics = Politics ?? skills.Politics;
        skills.Science = Science ?? skills.Science;
        skills.Technology = Technology ?? skills.Technology;
        if (character.Id is not null)
        {
            skills.Save((int)character.Id);
        }
        else
        {
            character.Save();
        }
    }
}

public class VampireDisciplinesResponse 
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Disciplines? Disciplines { get; set; }
}

public class VampireDisciplinesUpdate
{
    public string? School { get; set; }
    public int Score { get; set; }
    public void Apply(VampireV5Character character)
    {
        var disc = character.Disciplines;
        if (!string.IsNullOrEmpty(School)) {
            disc[School] = Score;
            if (character.Id is not null)
            {
                disc.Save((int)character.Id);
            }
            else
            {
                character.Save();
            }
        }
    }
}