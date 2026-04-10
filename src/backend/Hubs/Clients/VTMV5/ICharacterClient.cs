using Melpominee.app.Models.Web.VTMV5;
namespace Melpominee.app.Hubs.Clients.VTMV5;
public interface ICharacterClient
{
    Task WatcherUpdate(int charId, List<string> watchers);
    Task OnCharacterUpdate(int charId, string? updateId, DateTime timestamp, List<CharacterCommand> commands);
}
