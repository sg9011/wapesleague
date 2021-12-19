using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class AssociationTenantPlayerManager : IAssociationTenantPlayerManager
    {
        private readonly WaPesDbContext _context;
        public AssociationTenantPlayerManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<AssociationTenantPlayer>> GetAssociationTenantPlayersByAssociationTenantIdAsync(int associationTenantId)
        {
            return await _context.AssociationTenantPlayers
                .Include(atp => atp.User)
                    .ThenInclude(u => u.UserMembers)
                .Include(atp => atp.User)
                    .ThenInclude(u => u.PlatformUsers)
                        .ThenInclude(pu => pu.Platform)
                .Include(atp => atp.AssociationTeamPlayers)
                .AsNoTracking()
                .Where(atp => atp.AssociationTenantId == associationTenantId)
                .ToListAsync();
        }

        public async Task<List<AssociationTenantPlayer>> AddMultipleAsync(List<AssociationTenantPlayer> associationTenantPlayersToAdd)
        {
            await _context.AssociationTenantPlayers.AddRangeAsync(associationTenantPlayersToAdd);
            await _context.SaveChangesAsync();

            return associationTenantPlayersToAdd;
        }

        public async Task<IReadOnlyCollection<AssociationTenantPlayer>> GetAllAsync(int associationTenantId)
        {
            return await _context.AssociationTenantPlayers
                .Include(atp => atp.AssociationTeamPlayers)
                    .ThenInclude(atmp => atmp.AssociationTeam)
                .AsNoTracking()
                .Where(atp => atp.AssociationTenantId == associationTenantId)
                .ToListAsync();
        }
    }
}
