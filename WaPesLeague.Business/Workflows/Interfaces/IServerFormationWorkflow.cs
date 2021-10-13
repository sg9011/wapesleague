using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Formation;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Dto;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IServerFormationWorkflow
    {
        public Task<IReadOnlyCollection<FormationDto>> GetAllServerFormationsAsync(DiscordCommandPropsDto propsDto);
        public Task<DiscordWorkflowResult> AddServerFormationAsync(AddServerFormationDto addServerFormationDto);
        public Task<DiscordWorkflowResult> SetDefaultServerFormationAsync(string formationName, DiscordCommandPropsDto propsDto);
    }
}
