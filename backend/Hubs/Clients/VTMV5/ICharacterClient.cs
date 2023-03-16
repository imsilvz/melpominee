using Melpominee.app.Models.Web.VTMV5;
namespace Melpominee.app.Hubs.Clients.VTMV5;
public interface ICharacterClient
{
    Task OnHeaderUpdate(int charId, VampireCharacterUpdate update);
    Task OnAttributeUpdate(int charId, VampireAttributesUpdate update);
    Task OnSkillUpdate(int charId, VampireSkillsUpdate update);
}