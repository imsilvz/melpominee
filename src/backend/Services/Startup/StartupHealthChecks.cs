using Dapper;
using Melpominee.app.Services.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Melpominee.app.Services.Startup;

/// <summary>
/// Always-Healthy liveness probe target. Failure means the process is wedged
/// and k8s should restart.
/// </summary>
public sealed class LivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}

/// <summary>
/// Healthy iff the startup TCS has been marked ready. Reports a fault reason
/// (without stack traces) when <see cref="StartupReadiness.IsFaulted"/>.
/// Target of k8s startupProbe.
/// </summary>
public sealed class StartupHealthCheck : IHealthCheck
{
    private readonly StartupReadiness _readiness;

    public StartupHealthCheck(StartupReadiness readiness)
    {
        _readiness = readiness;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_readiness.IsReady)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        if (_readiness.IsFaulted)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(_readiness.FaultReason?.Message ?? "startup faulted"));
        }
        return Task.FromResult(HealthCheckResult.Unhealthy("startup pending"));
    }
}

/// <summary>
/// Healthy iff the readiness TCS is set, we're not draining, Redis is
/// connected, and <c>SELECT 1</c> succeeds. Caches the result for 2 seconds
/// to avoid hammering PG/Redis on every probe poll (critical — k8s defaults
/// to probe-every-5s but the KEDA interceptor can poll much faster).
/// </summary>
public sealed class ReadinessHealthCheck : IHealthCheck
{
    private static readonly TimeSpan CacheWindow = TimeSpan.FromSeconds(2);

    private readonly StartupReadiness _readiness;
    private readonly DataContext _dataContext;
    private IConnectionMultiplexer? _redis;

    private readonly object _cacheLock = new();
    private HealthCheckResult? _cachedResult;
    private DateTimeOffset _cachedAt = DateTimeOffset.MinValue;

    // Single public ctor. The Testing environment doesn't register Redis,
    // so the multiplexer is resolved via GetService (nullable) rather than
    // GetRequiredService to avoid a DI resolve failure. Tests that need a
    // different mux (or null) set it via the non-public InjectRedisForTesting
    // hook below so DI doesn't have to choose between ctor overloads.
    public ReadinessHealthCheck(
        StartupReadiness readiness,
        DataContext dataContext,
        IServiceProvider services)
    {
        _readiness = readiness;
        _dataContext = dataContext;
        _redis = services.GetService<IConnectionMultiplexer>();
    }

    /// <summary>
    /// Test-only hook for overriding the captured multiplexer reference
    /// without adding a second public constructor (which would make DI
    /// constructor selection ambiguous in production).
    /// </summary>
    internal void InjectRedisForTesting(IConnectionMultiplexer? redis)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Fast fail paths intentionally bypass the cache — these flip
        // instantly during drain/fault and should be reflected immediately.
        if (!_readiness.IsReady)
        {
            if (_readiness.IsFaulted)
            {
                return HealthCheckResult.Unhealthy(
                    _readiness.FaultReason?.Message ?? "startup faulted");
            }
            return HealthCheckResult.Unhealthy("startup pending");
        }
        if (_readiness.IsDraining)
        {
            return HealthCheckResult.Unhealthy("draining");
        }

        // Cached positive result (PG+Redis verified). 2s window matches the
        // polling interval of the KEDA interceptor's readiness check.
        lock (_cacheLock)
        {
            if (_cachedResult is not null &&
                DateTimeOffset.UtcNow - _cachedAt < CacheWindow)
            {
                return _cachedResult.Value;
            }
        }

        var result = await ProbeDependenciesAsync(cancellationToken);

        lock (_cacheLock)
        {
            _cachedResult = result;
            _cachedAt = DateTimeOffset.UtcNow;
        }
        return result;
    }

    private async Task<HealthCheckResult> ProbeDependenciesAsync(CancellationToken ct)
    {
        if (_redis is not null && !_redis.IsConnected)
        {
            return HealthCheckResult.Unhealthy("redis disconnected");
        }

        try
        {
            await using var conn = await _dataContext.ConnectAsync(ct);
            var result = await conn.ExecuteScalarAsync<int>("SELECT 1");
            if (result != 1)
            {
                return HealthCheckResult.Unhealthy("db probe returned unexpected value");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"db probe failed: {ex.Message}");
        }

        return HealthCheckResult.Healthy();
    }
}
