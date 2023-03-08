using System.IO;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Melpominee.app.Utilities;

// Load Secrets
SecretManager.Instance.LoadSecret("mail-secrets");

// Create Initial Data Schema
Directory.CreateDirectory("data");
using (var connection = new SqliteConnection("Data Source=data/melpominee.db"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText =
    @"
        CREATE TABLE IF NOT EXISTS melpominee_users (
            email TEXT NOT NULL PRIMARY KEY,
            password TEXT NOT NULL,
            activation_key TEXT,
            active BOOL DEFAULT false
        );
        CREATE TABLE IF NOT EXISTS melpominee_logins (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            user_email TEXT NOT NULL,
            timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY(user_email) REFERENCES melpominee_users(email)
        );
        CREATE TABLE IF NOT EXISTS melpominee_users_rescue (
            user_email TEXT NOT NULL PRIMARY KEY,
            rescue_key TEXT,
            requested_timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            completed_timestamp DATETIME,
            FOREIGN KEY(user_email) REFERENCES melpominee_users(email)
        );
        CREATE TABLE IF NOT EXISTS melpominee_characters (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            concept TEXT NOT NULL,
            chronicle TEXT NOT NULL,
            ambition TEXT NOT NULL,
            desire TEXT NOT NULL,
            sire TEXT NOT NULL,
            generation INTEGER NOT NULL,
            clan TEXT NOT NULL,
            predator_type TEXT NOT NULL,
            hunger INTEGER NOT NULL,
            resonance TEXT NOT NULL,
            blood_potency INTEGER NOT NULL
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_attributes (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            sheet_id INTEGER NOT NULL,
            attribute TEXT NOT NULL,
            score INTEGER NOT NULL,
            UNIQUE(sheet_id, attribute),
            FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_skills (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            sheet_id INTEGER NOT NULL,
            skill TEXT NOT NULL,
            speciality TEXT NOT NULL,
            score INTEGER NOT NULL,
            UNIQUE(sheet_id, skill),
            FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_secondary (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            sheet_id INTEGER NOT NULL,
            stat_name TEXT NOT NULL,
            base_value INTEGER NOT NULL,
            superficial_damage INTEGER NOT NULL,
            aggravated_damage INTEGER NOT NULL,
            UNIQUE(sheet_id, stat_name),
            FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_disciplines (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            sheet_id INTEGER NOT NULL,
            discipline TEXT NOT NULL,
            score INTEGER NOT NULL,
            UNIQUE(sheet_id, discipline),
            FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_discipline_powers (
            id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            sheet_id INTEGER NOT NULL,
            power_name TEXT NOT NULL,
            UNIQUE(sheet_id, power_name),
            FOREIGN KEY(sheet_id) REFERENCES melpominee_characters(id)
        );
    ";
    command.ExecuteNonQuery();
}

//var user = new Melpominee.app.Models.Auth.MelpomineeUser("rjyawger@me.com");
//user.BeginResetPassword("http://localhost:5173");
var sheet = new Melpominee.app.Models.CharacterSheets.VTMV5.VampireV5Sheet()//(1);
{
    Name = "Logan Bessett",
    Concept = "Schizo WWII Vet, also Therapist",
    Chronicle = "Gehenna Lost",
    Ambition = "Grant Sire True Death",
    Desire = "I forgot",
    Sire = "Alfred von Halstatt, M.D.",
    Generation = 12,
    Clan = Melpominee.app.Models.CharacterSheets.VTMV5.VampireClan.GetClan("malkavian"),
    PredatorType = Melpominee.app.Models.CharacterSheets.VTMV5.VampirePredatorType.GetPredatorType("sandman"),
    Attributes = {
        Strength = 2,
        Dexterity = 1,
        Stamina = 2,
        Charisma = 3,
        Manipulation = 3,
        Composure = 2,
        Intelligence = 2,
        Wits = 3,
        Resolve = 4
    },
    Skills = {
        Athletics = { Score = 2 },
        Brawl = { Score = 1 },
        Firearms = { Speciality = ".45 Pistol", Score = 3 },
        Stealth = { Score = 2 },
        Insight = { Score = 2 },
        Performance = { Speciality = "Piano", Score = 1 },
        Persuasion = { Score = 3 },
        Streetwise = { Score = 1 },
        Subterfuge = { Score = 2 },
        Awareness = { Score = 3 },
        Finance = { Score = 1 },
        Investigation = { Score = 2 },
        Medicine = { Speciality = "Anasthetics", Score = 1 },
        Occult = { Score = 1 },
        Politics = { Score = 1 },
    },
    Disciplines = new Dictionary<string, int>()
    {
        ["Auspex"] = 3,
        ["Dominate"] = 2,
        ["Obfuscate"] = 2,
    },
    DisciplinePowers = new List<Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower>()
    {
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("heightened_senses"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("premonition"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("scry_the_soul"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("cloud_memory"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("mesmerize"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("cloak_of_shadows"),
        Melpominee.app.Models.CharacterSheets.VTMV5.VampirePower.GetDisciplinePower("unseen_passage"),
    },
    Hunger = 1,
    Resonance = "Melancholic",
    BloodPotency = 1,
};
sheet.Save();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sheet));
var sheet2 = new Melpominee.app.Models.CharacterSheets.VTMV5.VampireV5Sheet(1);
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sheet2));
sheet2.Hunger = 5;
sheet2.Save();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sheet2));

// API Application Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();
