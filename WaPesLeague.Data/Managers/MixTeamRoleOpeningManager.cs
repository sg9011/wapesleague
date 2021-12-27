using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixTeamRoleOpeningManager : IMixTeamRoleOpeningManager
    {
        private readonly WaPesDbContext _context;
        public MixTeamRoleOpeningManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixTeamRoleOpening> AddAsync(MixTeamRoleOpening mixTeamRoleOpening)
        {
            await _context.MixTeamRoleOpenings.AddAsync(mixTeamRoleOpening);
            await _context.SaveChangesAsync();

            return mixTeamRoleOpening;
        }

        public async Task<List<MixTeamRoleOpening>> GetActiveMixTeamRoleOpeningsAsync()
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();
            return await _context.MixTeamRoleOpenings
                .Include(mtro => mtro.ServerRole)
                .AsNoTracking()
                .Where(mtro => dbNow < mtro.End)
                .ToListAsync();
        }
    }
}
