using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Utilities;
using Melpominee.app.Utilities.Database;
using Melpominee.app.Utilities.Hubs;
using Melpominee.app.Models.CharacterSheets.VTMV5;

// Load Secrets
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
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false";
});
builder.Services.AddSignalR()
    .AddStackExchangeRedis(
        $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false", 
        options => {
            options.Configuration.ChannelPrefix = "Melpominee";
        }
    );
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "Melpominee.app.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<ConnectionHelper>();

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
app.MapHub<CharacterHub>("/vtmv5/watch");
app.MapControllers();

app.Run();
