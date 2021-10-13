using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixTeamManager
    {
        public Task<MixTeam> AddAsync(MixTeam mixTeam);
        public Task<List<MixTeam>> AddMultipleAsync(List<MixTeam> mixTeams);
        public Task<MixTeam> UpdateAsync(MixTeam mixTeam);
    }
}
