using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class SniperManager : ISniperManager
    {
        private readonly WaPesDbContext _context;
        public SniperManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<Sniper> AddAsync(Sniper sniper)
        {
            await _context.Snipers.AddAsync(sniper);
            await _context.SaveChangesAsync();

            return sniper;
        }

        public async Task<List<Sniper>> GetAllActiveAsync()
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();
            return await _context.Snipers
                .Include(s => s.UserMember)
                .Where(s => s.DateEnd > dbNow)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
