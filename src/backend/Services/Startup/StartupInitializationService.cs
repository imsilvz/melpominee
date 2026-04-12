using System.Diagnostics;
using Melpominee.app.Services.Database;
using StackExchange.Redis;

namespace Melpominee.app.Services.Startup;

/// <summary>
/// Runs the backend's expensive startup work in parallel with Kestrel
/// accepting traffic. Secrets are loaded synchronously at the top of
/// <c>Program.cs</c> (several Redis-dependent DI registrations eagerly
/// evaluate secret values at registration time), so this service only
/// handles the parallel phases: schema DDL, Redis warm-ping, and pool warm.
/// Catches exceptions so the pod stays alive with readiness=false, letting
/// <c>startupProbe.failureThreshold</c> trigger a clean k8s restart.
/// </summary>
public sealed class StartupInitializationService : BackgroundService
{
    private readonly StartupReadiness _readiness;
    private readonly DataContext _dataContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<StartupInitializationService> _log;

    public StartupInitializationService(
        StartupReadiness readiness,
        DataContext dataContext,
        IConnectionMultiplexer redis,
        ILogger<StartupInitializationService> log)
    {
        _readiness = readiness;
        _dataContext = dataContext;
        _redis = redis;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var overall = Stopwatch.StartNew();
        try
        {
            await Task.WhenAll(
                RunPhaseAsync("schema",
                    () => _dataContext.InitializeAsync(ct), ct),
                RunPhaseAsync("redis",
                    () => WarmRedisAsync(ct), ct),
                RunPhaseAsync("pool",
                    () => _dataContext.WarmPoolAsync(ct), ct));

            _readiness.MarkReady();
            _log.LogInformation(
                "Startup complete in {ElapsedMs}ms",
                overall.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _log.LogError(ex,
                "Startup failed after {ElapsedMs}ms",
                overall.ElapsedMilliseconds);
            _readiness.MarkFaulted(ex);
            // Do NOT rethrow — unhandled BackgroundService exceptions stop
            // the host by default in .NET 6+. We want the pod alive with
            // readiness=false so startupProbe.failureThreshold triggers a
            // clean k8s restart.
        }
    }

    private async Task RunPhaseAsync(string phase, Func<Task> work, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        using var scope = _log.BeginScope(
            new Dictionary<string, object> { ["startup_phase"] = phase });
        _log.LogInformation("Phase {Phase} started", phase);
        try
        {
            await work();
            _log.LogInformation(
                "Phase {Phase} completed in {ElapsedMs}ms",
                phase, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _log.LogError(ex,
                "Phase {Phase} failed after {ElapsedMs}ms",
                phase, sw.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task WarmRedisAsync(CancellationToken ct)
    {
        var db = _redis.GetDatabase();
        await db.PingAsync();
    }
}
