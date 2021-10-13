using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class PositionGroupManager : IPositionGroupManager
    {
        private readonly WaPesDbContext _context;
        public PositionGroupManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<PositionGroup>> GetAllAsync()
        {
            return await _context.PositionGroups
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
