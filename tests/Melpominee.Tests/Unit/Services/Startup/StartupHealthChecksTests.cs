using System.Reflection;
using Melpominee.app.Services.Database;
using Melpominee.app.Services.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shouldly;
using StackExchange.Redis;
using NSubstitute;
namespace Melpominee.Tests.Unit.Services.Startup;

public class StartupHealthChecksTests
{
    private static void InvokeInternal(object target, string name, params object[] args)
    {
        var method = target.GetType()
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        method.ShouldNotBeNull();
        method!.Invoke(target, args);
    }

    private static ReadinessHealthCheck BuildCheck(StartupReadiness readiness)
    {
        var services = new ServiceCollection();
        services.AddSingleton(readiness);
        services.AddSingleton(new DataContext());
        // Intentionally do NOT register IConnectionMultiplexer — matches the
        // Testing env code path in Program.cs. The check's ctor should
        // tolerate the missing service via GetService.
        using var provider = services.BuildServiceProvider();
        return ActivatorUtilities.CreateInstance<ReadinessHealthCheck>(provider);
    }

    [Fact]
    public async Task ReadinessHealthCheck_PendingReturns503()
    {
        var readiness = new StartupReadiness();
        var check = BuildCheck(readiness);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Description.ShouldBe("startup pending");
    }

    [Fact]
    public async Task ReadinessHealthCheck_FaultedReturnsUnhealthy()
    {
        var readiness = new StartupReadiness();
        InvokeInternal(readiness, "MarkFaulted", new InvalidOperationException("kaboom"));

        var check = BuildCheck(readiness);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Description.ShouldBe("kaboom");
    }

    [Fact]
    public async Task ReadinessHealthCheck_DrainingReturnsUnhealthy()
    {
        var readiness = new StartupReadiness();
        InvokeInternal(readiness, "MarkReadyForTesting");
        InvokeInternal(readiness, "BeginDraining");

        var check = BuildCheck(readiness);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Description.ShouldBe("draining");
    }

    [Fact]
    public async Task StartupHealthCheck_PendingReturnsUnhealthy()
    {
        var readiness = new StartupReadiness();
        var check = new StartupHealthCheck(readiness);

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
        result.Description.ShouldBe("startup pending");
    }

    [Fact]
    public async Task StartupHealthCheck_ReadyReturnsHealthy()
    {
        var readiness = new StartupReadiness();
        InvokeInternal(readiness, "MarkReadyForTesting");

        var check = new StartupHealthCheck(readiness);
        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task LivenessHealthCheck_AlwaysHealthy()
    {
        var check = new LivenessHealthCheck();

        var result = await check.CheckHealthAsync(new HealthCheckContext());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }
}
