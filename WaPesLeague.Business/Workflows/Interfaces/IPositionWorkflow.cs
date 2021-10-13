using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Position;
using WaPesLeague.Business.Helpers;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IPositionWorkflow
    {
        public Task<IReadOnlyCollection<PositionDto>> GetAllPositionsOrderedAsync(int? serverId = null);
        public Task<DiscordWorkflowResult> AddPositionTagAsync(string positionCode, string tag, int serverId);
        public Task<DiscordWorkflowResult> RemovePositionTagAsync(string tag, int serverId);
        public Task<DiscordWorkflowResult> SetPositionDisplayAsync(string positionCode, string displayValue, int serverId);
    }
}
