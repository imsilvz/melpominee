using Melpominee.app.Models.Web.VTMV5;
namespace Melpominee.app.Hubs.Clients.VTMV5;
public interface ICharacterClient
{
    Task OnHeaderUpdate(int charId, DateTime timestamp, VampireCharacterUpdate update);
    Task OnAttributeUpdate(int charId, DateTime timestamp, VampireAttributesUpdate update);
    Task OnSkillUpdate(int charId, DateTime timestamp, VampireSkillsUpdate update);
    Task OnSecondaryUpdate(int charId, DateTime timestamp, VampireStatsUpdate update);
    Task OnDisciplineUpdate(int charId, DateTime timestamp, VampireDisciplinesUpdate update);
    Task OnPowerUpdate(int charId, DateTime timestamp, VampirePowersUpdate update);
    Task OnBeliefsupdate(int charId, DateTime timestamp, VampireBeliefsUpdate update);
    Task OnBackgroundMeritFlawUpdate(int charId, DateTime timestamp, VampireBackgroundMeritFlawUpdate update);
    Task OnProfileUpdate(int charId, DateTime timestamp, VampireProfileUpdate update);
}