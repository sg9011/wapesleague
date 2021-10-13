using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Mix;

namespace WaPesLeague.Business.Workflows.Interfaces
{
    public interface IMixStatsWorkflow
    {
        public Task<IReadOnlyCollection<UserPositionGroupStatDto>> GetUserAdvancedStats(int userId, int? serverId = null);
        public Task HandleCalculatedNotCalculatedSessions();
    }
}
