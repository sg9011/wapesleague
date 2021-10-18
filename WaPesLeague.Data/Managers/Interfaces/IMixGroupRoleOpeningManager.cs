using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;

namespace WaPesLeague.Data.Managers.Interfaces
{
    public interface IMixGroupRoleOpeningManager
    {
        public Task<MixGroupRoleOpening> AddAsync(MixGroupRoleOpening mixGroupRoleOpening);
        public Task<bool> DeActivateAsync(int MixGroupRoleOpeningId); //Set DateEnd and isActive = false
        public Task<List<MixGroupRoleOpening>> GetActiveMixGroupRoleOpenings();
        
    }
}
