using Microsoft.AspNetCore.SignalR;
namespace Melpominee.app.Hubs;

public class CharacterHub : Hub
{
    public async Task NewMessage(long username, string message)
    {
        await Clients.All.SendAsync("messageReceived", username, message);
    }
}