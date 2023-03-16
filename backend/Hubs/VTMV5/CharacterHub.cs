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
    private readonly ConnectionHelper _connectionHelper;
    public CharacterHub(ConnectionHelper connectionHelper)
    {
        _connectionHelper = connectionHelper;
    }

    public async Task WatchCharacter(int charId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"character_{charId}");
    }

    public override Task OnConnectedAsync()
    {
        _connectionHelper.OnConnect(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionHelper.OnDisconnect(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}