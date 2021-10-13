using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixChannelTeamTagManager
    {
        public Task<MixChannelTeamTag> AddAsync(MixChannelTeamTag mixChannelTeamTag);
        public Task<List<MixChannelTeamTag>> AddMultipleAsync(List<MixChannelTeamTag> mixChannelTeamTags);
    }
}
