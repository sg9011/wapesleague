using System.Threading.Tasks;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IDiscordWorkflow
    {
        public Task HandleScanForMembersAsync();
    }
}
