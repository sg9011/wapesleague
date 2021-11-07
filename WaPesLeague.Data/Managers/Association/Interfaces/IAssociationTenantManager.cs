using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;

namespace WaPesLeague.Data.Managers.Association.Interfaces
{
    public interface IAssociationTenantManager
    {
        public Task<IReadOnlyCollection<AssociationTenant>> GetAllAsync();
        public Task<AssociationTenant> AddAsync(AssociationTenant accociationTenant);
    }
}
