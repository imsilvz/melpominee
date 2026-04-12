using System.Diagnostics;
using System.Reflection;
using Melpominee.app.Services.Startup;
using Shouldly;
namespace Melpominee.Tests.Unit.Services.Startup;

public class StartupReadinessTests
{
    // Helper: the public surface keeps MarkReady / MarkFaulted / BeginDraining
    // / MarkReadyForTesting internal so Program.cs alone can mutate them.
    // Tests reach through reflection rather than bumping the visibility.
    private static void InvokeInternal(StartupReadiness target, string name, params object[] args)
    {
        var method = typeof(StartupReadiness)
            .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        method.ShouldNotBeNull();
        method!.Invoke(target, args);
    }

    [Fact]
    public async Task StartupReadiness_ConcurrentWaiters_AllUnblock()
    {
        var readiness = new StartupReadiness();
        const int waiterCount = 50;
        var waiters = new Task[waiterCount];
        for (int i = 0; i < waiterCount; i++)
        {
            waiters[i] = readiness.WhenReadyAsync();
        }

        // Mark ready after all waiters are registered, then measure how
        // long it takes every continuation to complete.
        var sw = Stopwatch.StartNew();
        InvokeInternal(readiness, "MarkReady");
        await Task.WhenAll(waiters).WaitAsync(TimeSpan.FromSeconds(5));
        sw.Stop();

        readiness.IsReady.ShouldBeTrue();
        sw.ElapsedMilliseconds.ShouldBeLessThan(100);
    }

    [Fact]
    public async Task StartupReadiness_Faulted_WaitersThrow()
    {
        var readiness = new StartupReadiness();
        var sentinel = new InvalidOperationException("boom");

        var waiter = readiness.WhenReadyAsync();
        InvokeInternal(readiness, "MarkFaulted", sentinel);

        var thrown = await Should.ThrowAsync<Exception>(() => waiter);
        readiness.IsFaulted.ShouldBeTrue();
        readiness.IsReady.ShouldBeFalse();
        // The TCS wraps the original exception in an AggregateException; both
        // unwrapped .InnerException and the original instance are acceptable.
        (thrown == sentinel || thrown.InnerException == sentinel || thrown.Message.Contains("boom"))
            .ShouldBeTrue();
        readiness.FaultReason.ShouldBe(sentinel);
    }

    [Fact]
    public async Task StartupReadiness_MarkReadyForTesting_Completes()
    {
        var readiness = new StartupReadiness();
        readiness.IsReady.ShouldBeFalse();

        InvokeInternal(readiness, "MarkReadyForTesting");

        readiness.IsReady.ShouldBeTrue();
        readiness.IsDraining.ShouldBeFalse();
        // Should not throw — already completed.
        await readiness.WhenReadyAsync().WaitAsync(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void StartupReadiness_BeginDraining_FlipsFlag()
    {
        var readiness = new StartupReadiness();
        readiness.IsDraining.ShouldBeFalse();

        InvokeInternal(readiness, "BeginDraining");

        readiness.IsDraining.ShouldBeTrue();
    }
}
