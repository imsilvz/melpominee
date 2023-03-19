using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.Clients.VTMV5;
using Melpominee.app.Utilities.Hubs;
namespace Melpominee.app.Hubs.VTMV5;

[Authorize]
public class CharacterHub : Hub<ICharacterClient>
{
    private readonly IServiceProvider _provider;
    private readonly ConnectionHelper _connectionHelper;
    public CharacterHub(IServiceProvider provider, ConnectionHelper connectionHelper)
    {
        _provider = provider;
        _connectionHelper = connectionHelper;
    }

    public async Task WatchCharacter(int charId)
    {
        using (var scope = _provider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var hub = scopedServices.GetRequiredService<IHubContext<CharacterHub, ICharacterClient>>();

            var groupId = $"character_{charId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, $"character_{charId}");

            // alert group of new watcher!
            _connectionHelper.OnGroupAdd(hub, Context.ConnectionId, groupId);
            var connList = _connectionHelper.GroupMap[groupId].Keys.ToList();
            var watcherDict = new Dictionary<string, bool>();
            foreach(var conn in connList)
                watcherDict.Add(_connectionHelper.GetConnectedUser(conn), true);
            await Clients.Group(groupId).WatcherUpdate(charId, watcherDict.Keys.ToList());
        }
    }

    public override Task OnConnectedAsync()
    {
        using (var scope = _provider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var hub = scopedServices.GetRequiredService<IHubContext<CharacterHub, ICharacterClient>>();

            _connectionHelper.OnConnect(hub, Context.UserIdentifier!, Context.ConnectionId);
            return base.OnConnectedAsync();
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        using (var scope = _provider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var hub = scopedServices.GetRequiredService<IHubContext<CharacterHub, ICharacterClient>>();

            _connectionHelper.OnDisconnect(hub, Context.UserIdentifier!, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}