using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Dto;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IMixGroupWorkflow
    {
        public Task<DiscordWorkflowResult> CreateMixGroupAsync(CreateMixRoomGroupDto dto);
        public Task HandleAutoCloseAndRecreateOfMixSesisons();
        public Task<DiscordWorkflowResult> DeActivateMixGroupAsync(DiscordCommandPropsDto propsDto);
    }
}
