using Melpominee.app.Models.Characters.VTMV5;
namespace Melpominee.app.Hubs.Clients.VTMV5;
public interface ICharacterClient
{
    Task WatcherUpdate(int charId, List<string> watchers);
    Task OnHeaderUpdate(int charId, string? updateId, DateTime timestamp, VampireCharacterUpdate update);
    Task OnAttributeUpdate(int charId, string? updateId, DateTime timestamp, VampireAttributeUpdate update);
    Task OnSkillUpdate(int charId, string? updateId, DateTime timestamp, VampireSkillUpdate update);
    Task OnSecondaryUpdate(int charId, string? updateId, DateTime timestamp, VampireStatsUpdate update);
    Task OnDisciplineUpdate(int charId, string? updateId, DateTime timestamp, VampireDisciplineUpdate update);
    Task OnPowersUpdate(int charId, string? updateId, DateTime timestamp, VampirePowersUpdate update);
    Task OnBeliefsupdate(int charId, string? updateId, DateTime timestamp, VampireBeliefsUpdate update);
    Task OnBackgroundMeritFlawUpdate(int charId, string? updateId, DateTime timestamp, VampireBackgroundMeritFlawUpdate update);
    Task OnProfileUpdate(int charId, string? updateId, DateTime timestamp, VampireProfileUpdate update);
}