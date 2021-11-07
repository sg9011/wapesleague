using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class AssociationTenantManager : IAssociationTenantManager
    {
        private readonly WaPesDbContext _context;
        public AssociationTenantManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<AssociationTenant>> GetAllAsync()
        {
            return await _context.AssociationTenants.AsNoTracking().ToListAsync();
        }

        public async Task<AssociationTenant> AddAsync(AssociationTenant associationTenant)
        {
            await _context.AssociationTenants.AddAsync(associationTenant);
            await _context.SaveChangesAsync();

            return associationTenant;
        }


    }
}
