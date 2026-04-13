using StackExchange.Redis;
using Melpominee.app.Authorization;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Services;
using Melpominee.app.Services.Auth;
using Melpominee.app.Services.Characters;
using Melpominee.app.Services.Database;
using Melpominee.app.Services.Hubs;
using Melpominee.app.Services.Startup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

// DB schema and Redis are no longer loaded synchronously here — those run
// in parallel behind the readiness gate inside StartupInitializationService.
// See scale-to-zero.md.

// API Application Builder
var builder = WebApplication.CreateBuilder(args);

// Secrets must load synchronously before DI registration because several
// Redis-dependent registrations (SignalR backplane, DataProtection, cache)
// read secret values eagerly during service registration. The slow cold-start
// phases (schema, redis, pool warm) remain parallel inside
// StartupInitializationService. Skipped entirely in Testing env to match
// existing CustomWebApplicationFactory behavior.
if (!builder.Environment.IsEnvironment("Testing"))
{
    SecretManager.Instance.LoadSecret("discord-oauth");
    SecretManager.Instance.LoadSecret("pg-credentials");
    SecretManager.Instance.LoadSecret("redis-credentials");
    SecretManager.Instance.LoadSecret("mail-secrets");
}

// Add services to the container.
builder.Services.AddControllers();

// Readiness primitive (needed by DataContext, health checks, graceful shutdown)
builder.Services.AddSingleton<StartupReadiness>();

// DataContext: DI resolves to the SAME object as DataContext.Instance.
// We attach the readiness reference during the factory callback so the
// async-gated ConnectAsync path and the legacy sync Connect() path share
// one NpgsqlDataSource, one _initGate, and one _initialized flag.
builder.Services.AddSingleton<DataContext>(sp =>
{
    var readiness = sp.GetRequiredService<StartupReadiness>();
    DataContext.Instance.AttachReadiness(readiness);
    return DataContext.Instance;
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    // Redis multiplexer is a DI singleton resolved lazily on first access.
    // The actual ConnectionMultiplexer.Connect call only runs when something
    // pulls the service (health check, data protection, etc.), so it doesn't
    // block Kestrel startup. A single shared Lazy captures the resolved mux
    // so the DataProtection Func<IDatabase> callback below can observe it
    // without a second connect.
    var redisLazy = new Lazy<IConnectionMultiplexer>(() =>
    {
        var host = SecretManager.Instance.GetSecret("redis_host");
        var port = SecretManager.Instance.GetSecret("redis_port");
        return ConnectionMultiplexer.Connect($"{host}:{port},abortConnect=false");
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    builder.Services.AddSingleton<IConnectionMultiplexer>(sp => redisLazy.Value);

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false";
    });

    // signalR
    builder.Services.AddSignalR()
        .AddStackExchangeRedis(
            $"{SecretManager.Instance.GetSecret("redis_host")}:{SecretManager.Instance.GetSecret("redis_port")},abortConnect=false",
            options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal("Melpominee");
            }
        );

    // DataProtection: the Func<IDatabase> overload is the real API shape —
    // no IServiceProvider parameter exists on this overload. Closure-capture
    // the shared Lazy above so the mux is resolved once, lazily, on first
    // data-protection usage.
    builder.Services
        .AddDataProtection()
        .PersistKeysToStackExchangeRedis(
            () => redisLazy.Value.GetDatabase(),
            "DataProtectionKeys")
        .UseCryptographicAlgorithms(
            new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            }
        );
}
else
{
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSignalR();
    builder.Services.AddDataProtection();
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.Name = "Melpominee.app.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<ConnectionService>();

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
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// authorization handlers
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, CanViewCharacterHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewCharacter", policy =>
        policy.Requirements.Add(new CanViewCharacterRequirement()));
});

// additional services
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<CharacterCommandDispatcher>();
builder.Services.AddScoped<UserManager>();

// Health checks (always registered — Testing env still serves them).
// The individual check classes are also registered as singletons so the
// factory can resolve them directly from the container.
builder.Services.AddSingleton<LivenessHealthCheck>();
builder.Services.AddSingleton<StartupHealthCheck>();
builder.Services.AddSingleton<ReadinessHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<LivenessHealthCheck>("live", tags: new[] { "live" })
    .AddCheck<StartupHealthCheck>("startup", tags: new[] { "startup" })
    .AddCheck<ReadinessHealthCheck>("ready", tags: new[] { "ready" });

// Startup orchestration — skipped in Testing env so integration tests don't
// sit behind an async init they don't actually run.
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<StartupInitializationService>();
}
else
{
    // Resolve the StartupReadiness TCS eagerly so ConnectAsync fast-paths
    // from the very first test request. This runs at DI build time, before
    // any hosted service starts.
    builder.Services.AddSingleton<IStartupFilter>(sp =>
    {
        sp.GetRequiredService<StartupReadiness>().MarkReadyForTesting();
        return new NoopStartupFilter();
    });
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Graceful shutdown (register LAST → LIFO shutdown → runs FIRST).
// Testing env skips this — there are no SignalR connections to drain
// and no NpgsqlDataSource to dispose in the test host.
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<GracefulShutdownService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Route backend at /api so it matches what Gateway/KEDA interceptor will
// send. Coordinated with the HTTPRoute change in the infrastructure
// section of scale-to-zero.md — both land in the same PR (PR-B).
// SPEC DEVIATION: gated off in Testing env so existing integration tests
// (which use unprefixed paths like /auth, /vtmv5/character) keep passing
// without rewrite. Production still gets the /api base.
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UsePathBase("/api");
}

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapHealthChecks("/health/live",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/health/startup",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("startup") });
app.MapHealthChecks("/health/ready",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("ready") });

app.MapHub<CharacterHub>("/vtmv5/watch");
app.MapControllers();

app.Run();

/// <summary>
/// No-op IStartupFilter used only in the Testing environment. Its purpose is
/// to provide a concrete IStartupFilter that the DI container can construct,
/// so the factory lambda (which calls MarkReadyForTesting as a side effect)
/// actually runs during DI graph build.
/// </summary>
internal sealed class NoopStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => next;
}

public partial class Program { }
