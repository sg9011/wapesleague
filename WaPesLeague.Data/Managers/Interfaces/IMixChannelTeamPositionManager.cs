using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixChannelTeamPositionManager
    {
        public Task<MixChannelTeamPosition> AddAsync(MixChannelTeamPosition mixChannelTeamPosition);
        public Task<List<MixChannelTeamPosition>> AddMultipleAsync(List<MixChannelTeamPosition> mixChannelTeamPositions);
    }
}
