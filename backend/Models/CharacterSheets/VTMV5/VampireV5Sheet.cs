namespace Melpominee.app.Models.CharacterSheets.VTMV5;

public class VampireV5Sheet : BaseCharacterSheet
{
    // header items
    public string Name { get; set; } = "";
    public string Concept { get; set; } = "";
    public string Chronicle { get; set; } = ""; // inherit from GameId
    public string Ambition { get; set; } = "";
    public string Desire { get; set; } = "";
    public string Sire { get; set; } = "";
    public int Generation { get; set; } = 13;
    public VampireClan? Clan { get; set; }
    public VampirePredatorType? PredatorType { get; set; }

    // primary stats
    public VampireV5Attributes Attributes { get; set; } = new VampireV5Attributes();
    public VampireV5Skills Skills { get; set; } = new VampireV5Skills();
    public VampireV5SecondaryStats SecondaryStats { get; set; } = new VampireV5SecondaryStats();

    // disciplines
    public List<VampireDiscipline> Disciplines { get; set; } = new List<VampireDiscipline>();

    public VampireV5Sheet() : base()
    {
        GameType = "VTMV5";
    }

    public VampireV5Sheet(int id) : base(id)
    {
        GameType = "VTMV5";
    }

    public override bool Load()
    {
        throw new NotImplementedException();
    }
}

public class VampireV5Attributes
{
    public int Strength { get; set; } = 0;
    public int Dexterity { get; set; } = 0;
    public int Stamina { get; set; } = 0;
    public int Charisma { get; set; } = 0;
    public int Manipulation { get; set; } = 0;
    public int Composure { get; set; } = 0;
    public int Intelligence { get; set; } = 0;
    public int Wits { get; set; } = 0;
    public int Resolve { get; set; } = 0;
}

public class VampireV5Skill
{
    public string Speciality { get; set; } = "";
    public int Score { get; set; } = 0;
}

public class VampireV5Skills
{
    public VampireV5Skill Athletics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Brawl { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Craft { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Drive { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Firearms { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Melee { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Larceny { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Stealth { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Survival { get; set; } = new VampireV5Skill {};
    public VampireV5Skill AnimalKen { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Ettiquette { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Insight { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Intimidation { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Leadership { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Performance { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Persuasion { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Streetwise { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Subterfuge { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Academics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Awareness { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Finance { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Investigation { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Medicine { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Occult { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Politics { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Science { get; set; } = new VampireV5Skill {};
    public VampireV5Skill Technology { get; set; } = new VampireV5Skill {};
}

public class VampireV5SecondaryStat
{
    public int BaseValue { get; set; } = 0;
    public int SuperficialDamage { get; set; } = 0;
    public int AggravatedDamage { get; set; } = 0;
}

public class VampireV5SecondaryStats
{
    public VampireV5SecondaryStat Health { get; set; } = new VampireV5SecondaryStat();
    public VampireV5SecondaryStat Willpower { get; set; } = new VampireV5SecondaryStat();
    public VampireV5SecondaryStat Humanity { get; set; } = new VampireV5SecondaryStat
    {
        BaseValue = 7,
    };
}