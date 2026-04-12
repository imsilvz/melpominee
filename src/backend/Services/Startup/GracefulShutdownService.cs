using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Melpominee.app.Services.Startup;

/// <summary>
/// Drains SignalR connections during pod termination. Implements
/// <see cref="IHostedService"/> (not <see cref="BackgroundService"/>) because
/// we only need the StopAsync hook. Registered LAST in DI so LIFO shutdown
/// runs it FIRST, before the hub is torn down.
/// </summary>
public sealed class GracefulShutdownService : IHostedService
{
    private readonly StartupReadiness _readiness;
    private readonly ConnectionService _connectionService;
    private readonly IHubContext<CharacterHub, ICharacterClient> _hub;
    private readonly ILogger<GracefulShutdownService> _log;

    public GracefulShutdownService(
        StartupReadiness readiness,
        ConnectionService connectionService,
        IHubContext<CharacterHub, ICharacterClient> hub,
        ILogger<GracefulShutdownService> log)
    {
        _readiness = readiness;
        _connectionService = connectionService;
        _hub = hub;
        _log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("Graceful shutdown: begin draining");

        // 1. Flip readiness to draining so the readiness probe returns 503
        //    immediately. This is the signal for k8s/KEDA/Gateway to stop
        //    routing new traffic to this pod.
        _readiness.BeginDraining();

        // 2. Let kube-proxy/Gateway pick up the unready state and remove us
        //    from their endpoints before we start closing open connections.
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // grace period cut short; proceed with drain
        }

        // 3. Tell connected clients to begin reconnect backoff now, before
        //    the WebSocket is force-closed on pod termination.
        try
        {
            await _hub.Clients.All.ServerShuttingDown();
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Graceful shutdown: ServerShuttingDown broadcast failed");
        }

        // 4. Poll the connection map until it's empty or 20s elapses.
        try
        {
            await _connectionService.DrainAsync(TimeSpan.FromSeconds(20), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // timed out waiting for drain; host will force-close remaining sockets
        }

        _log.LogInformation("Graceful shutdown: drain complete");
    }
}
