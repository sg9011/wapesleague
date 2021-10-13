using DSharpPlus;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IMixChannelWorkflow
    {
        public Task<DiscordWorkflowResult> CreateChannelForMixGroupAsync(MixGroup mixGroup, CreateMixRoomGroupDto dto);
        public Task<bool> CreateFollowUpForMixSessionIdAsync(DiscordClient client, int mixGroupId);

    }
}
