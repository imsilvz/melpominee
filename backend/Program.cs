using Melpominee.app.Utilities;
using Melpominee.app.Utilities.Database;
using Melpominee.app.Models.CharacterSheets.VTMV5;

// Load Secrets
SecretManager.Instance.LoadSecret("mail-secrets");

// Create Initial Data Schema
Directory.CreateDirectory("data");
DataContext.Instance.Initalize();
//var user = new Melpominee.app.Models.Auth.MelpomineeUser("rjyawger@me.com");
//user.BeginResetPassword("http://localhost:5173");
var sheet = new VampireV5Character()
{
    Name = "Logan Bessett",
    Concept = "Schizo WWII Vet, also Therapist",
    Chronicle = "Gehenna Lost",
    Ambition = "Grant Sire True Death",
    Desire = "I forgot",
    Sire = "Alfred von Halstatt, M.D.",
    Generation = 12,
    Clan = VampireClan.GetClan("malkavian"),
    PredatorType = VampirePredatorType.GetPredatorType("sandman"),
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
    Disciplines = new VampireV5Disciplines()
    {
        ["Auspex"] = 3,
        ["Dominate"] = 2,
        ["Obfuscate"] = 2,
    },
    DisciplinePowers = new VampireV5DisciplinePowers()
    {
        VampirePower.GetDisciplinePower("heightened_senses"),
        VampirePower.GetDisciplinePower("premonition"),
        VampirePower.GetDisciplinePower("scry_the_soul"),
        VampirePower.GetDisciplinePower("cloud_memory"),
        VampirePower.GetDisciplinePower("mesmerize"),
        VampirePower.GetDisciplinePower("cloak_of_shadows"),
        VampirePower.GetDisciplinePower("unseen_passage"),
    },
    Hunger = 3,
    Resonance = "Melancholic",
    BloodPotency = 3,
};
sheet.Save();

// API Application Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "Melpominee.app.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// authentication details
const string CookieScheme = "Melpominee.app.Auth";
builder.Services.AddAuthentication(CookieScheme)
    .AddCookie(CookieScheme, options =>
    {
        options.Cookie.Name = CookieScheme;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.Clear();
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context => 
        {
            context.Response.Clear();
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();
