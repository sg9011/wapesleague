using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IAssociationTenantPlayerManager
    {
        public Task<IReadOnlyCollection<AssociationTenantPlayer>> GetAssociationTenantPlayersByAssociationTenantIdAsync(int associationTenantId);
        public Task<List<AssociationTenantPlayer>> AddMultipleAsync(List<AssociationTenantPlayer> associationTenantPlayersToAdd);
        public Task<IReadOnlyCollection<AssociationTenantPlayer>> GetAllAsync(int associationTenantId);
    }
}
