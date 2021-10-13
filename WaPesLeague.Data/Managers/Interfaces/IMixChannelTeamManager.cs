using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixChannelTeamManager
    {
        public Task<MixChannelTeam> AddAsync(MixChannelTeam mixChannelTeam);
        public Task<List<MixChannelTeam>> AddMultipleAsync(List<MixChannelTeam> mixChannelTeams);
    }
}
