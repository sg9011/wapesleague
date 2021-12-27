using System.Threading.Tasks;
using WaPesLeague.Business.Dto;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Data.Entities.Discord;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IServerButtonWorkflow
    {
        public Task<DiscordWorkflowResult> HandleGetServerButtonsAsync(Server server);
        public Task<DiscordWorkflowResult> HandleAddServerButtonAsync(DiscordCommandPropsDto props, string options);
        public Task<DiscordWorkflowResult> HandleDeleteServerButtonAsync(Server server, int buttonId);

    }
}
