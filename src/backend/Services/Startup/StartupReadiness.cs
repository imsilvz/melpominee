namespace Melpominee.app.Services.Startup;

/// <summary>
/// Coordination primitive that gates request processing on the completion of
/// the backend's non-blocking startup initialization. Registered as a DI
/// singleton. Backed by a <see cref="TaskCompletionSource"/> constructed with
/// <see cref="TaskCreationOptions.RunContinuationsAsynchronously"/> so
/// <see cref="MarkReady"/> does not execute awaiter continuations on the
/// init thread.
/// </summary>
public sealed class StartupReadiness
{
    private readonly TaskCompletionSource _tcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _draining;

    /// <summary>
    /// Awaits until startup marks itself ready. Fast-paths when already
    /// complete. Honors the supplied cancellation token.
    /// </summary>
    public Task WhenReadyAsync(CancellationToken ct = default) =>
        _tcs.Task.IsCompleted ? _tcs.Task : _tcs.Task.WaitAsync(ct);

    public bool IsReady => _tcs.Task.Status == TaskStatus.RanToCompletion;
    public bool IsFaulted => _tcs.Task.IsFaulted;
    public Exception? FaultReason => _tcs.Task.Exception?.InnerException;

    /// <summary>
    /// True once <see cref="BeginDraining"/> has been called. The readiness
    /// health check consults this to flip to Unhealthy as soon as graceful
    /// shutdown begins, so k8s + KEDA stop routing new traffic to this pod.
    /// </summary>
    public bool IsDraining => _draining;

    internal void MarkReady() => _tcs.TrySetResult();

    internal void MarkFaulted(Exception ex) => _tcs.TrySetException(ex);

    /// <summary>
    /// Flips the draining flag on. Intended to be called exactly once from
    /// <c>GracefulShutdownService.StopAsync</c>.
    /// </summary>
    internal void BeginDraining() => _draining = true;

    /// <summary>
    /// Used by the Testing-environment branch in Program.cs to resolve the
    /// TCS synchronously during DI setup so integration tests (which mock
    /// DataContext / UserManager via NSubstitute) never sit behind an async
    /// init they don't actually run.
    /// </summary>
    internal void MarkReadyForTesting() => _tcs.TrySetResult();
}
