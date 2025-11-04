using StackExchange.Redis;
using Melpominee.app.Authorization;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Services;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using Melpominee.app.Services.Database;
using Melpominee.app.Services.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

// Load Secrets
SecretManager.Instance.LoadSecret("discord-oauth");
SecretManager.Instance.LoadSecret("pg-credentials");
SecretManager.Instance.LoadSecret("redis-credentials");
SecretManager.Instance.LoadSecret("mail-secrets");

// Create Initial Data Schema
Directory.CreateDirectory("data");
DataContext.Instance.Initalize();

// API Application Builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
/*
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false";
});
*/

// signalR
/*
builder.Services.AddSignalR()
    .AddStackExchangeRedis(
        $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false",
        options =>
        {
            options.Configuration.ChannelPrefix = "Melpominee";
        }
    );
*/
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.Name = "Melpominee.app.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<ConnectionService>();

// add data protection
/*
var redis = ConnectionMultiplexer
    .Connect($"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false");
builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys")
    .UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        }
    );
*/

// authentication details
const string CookieScheme = "Melpominee.app.Auth.V2";
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
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// authorization handlers
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, CanViewCharacterHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewCharacter", policy =>
        policy.Requirements.Add(new CanViewCharacterRequirement()));
});

// additional services
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<UserManager>();

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
//app.MapHub<CharacterHub>("/vtmv5/watch");
app.MapControllers();

app.Run();
