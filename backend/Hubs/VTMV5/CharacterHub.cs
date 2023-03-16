using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.Clients.VTMV5;
namespace Melpominee.app.Hubs.VTMV5;

public class CharacterHub : Hub<ICharacterClient>
{
    public async Task WatchCharacter(int charId)
    {
        Console.WriteLine(Context.User);
        Console.WriteLine(Context.User?.Identity?.Name);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"character_{charId}");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}