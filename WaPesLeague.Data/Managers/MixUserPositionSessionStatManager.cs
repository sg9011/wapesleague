using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixUserPositionSessionStatManager : IMixUserPositionSessionStatManager
    {
        private readonly WaPesDbContext _context;
        public MixUserPositionSessionStatManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<List<MixUserPositionSessionStat>> AddMultipleAsync(List<MixUserPositionSessionStat> mixUserPositionSessionStats)
        {
            await _context.MixUserPositionSessionStats.AddRangeAsync(mixUserPositionSessionStats);
            await _context.SaveChangesAsync();

            return mixUserPositionSessionStats;
        }

        public async Task<List<MixUserPositionSessionStat>> GetAllAsync()
        {
            return await _context.MixUserPositionSessionStats
                .Include(x => x.Position.ParentPosition)
                .Include(x => x.Position.PositionGroup)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
