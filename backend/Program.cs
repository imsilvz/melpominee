using System.IO;
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
    ";
    command.ExecuteNonQuery();
}

//var user = new Melpominee.app.Models.Auth.MelpomineeUser("rjyawger@me.com");
//user.BeginResetPassword("http://localhost:5173");
var sheet = new Melpominee.app.Models.CharacterSheets.VTMV5.VampireV5Sheet()
{
    Name = "Logan Bessett",
    Concept = "Schizo WWII Vet, also Therapist",
    Chronicle = "Gehenna Lost",
    Ambition = "Grant Sire True Death",
    Desire = "I forgot",
    Sire = "Alfred von Halstatt, M.D.",
    Generation = 12,
    Clan = Melpominee.app.Models.CharacterSheets.VTMV5.VampireClan.GetClan("malkavian"),
    PredatorType = Melpominee.app.Models.CharacterSheets.VTMV5.VampirePredatorType.GetPredatorType("sandman")
};
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sheet));

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
