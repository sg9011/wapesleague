using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixTeamRoleOpeningManager
    {
        public Task<MixTeamRoleOpening> AddAsync(MixTeamRoleOpening mixTeamRoleOpening);
        public Task<List<MixTeamRoleOpening>> GetActiveMixTeamRoleOpeningsAsync();
    }
}
