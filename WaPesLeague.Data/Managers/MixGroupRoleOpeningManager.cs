using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixGroupRoleOpeningManager : IMixGroupRoleOpeningManager
    {
        private readonly WaPesDbContext _context;
        public MixGroupRoleOpeningManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixGroupRoleOpening> AddAsync(MixGroupRoleOpening mixGroupRoleOpening)
        {
            await _context.MixGroupRoleOpenings.AddAsync(mixGroupRoleOpening);
            await _context.SaveChangesAsync();

            return mixGroupRoleOpening;
        }

        public async Task<bool> DeActivateAsync(int MixGroupRoleOpeningId)
        {
            var currentMixGroupRoleOpening = await _context.MixGroupRoleOpenings.FindAsync(MixGroupRoleOpeningId);
            if (currentMixGroupRoleOpening != null)
            {
                currentMixGroupRoleOpening.IsActive = false;
                currentMixGroupRoleOpening.DateEnd = DateTimeHelper.GetDatabaseNow();
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MixGroupRoleOpening>> GetActiveMixGroupRoleOpenings()
        {
            return await _context.MixGroupRoleOpenings
                .Include(mgro => mgro.MixGroup)
                    .ThenInclude(mg => mg.Server)
                .Include(mgro => mgro.ServerRole)
                .AsNoTracking()
                .Where(mgro => mgro.IsActive == true)
                .ToListAsync();

        }
    }
}
