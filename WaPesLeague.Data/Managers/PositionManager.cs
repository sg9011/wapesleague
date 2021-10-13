using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class PositionManager : IPositionManager
    {
        private readonly WaPesDbContext _context;
        public PositionManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyCollection<Position>> GetAllPositionsAsync(int? serverId)
        {
            return await _context.Positions
                .Include(p => p.PositionGroup)
                .Include(p => p.ParentPosition)
                .Include(p => p.Tags.Where(t => serverId != null && t.ServerId == serverId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Position> GetPostionByTagOrCodeAsync(string positionTagOrCode, int? serverId)
        {
            return await _context.Positions
                .Include(p => p.Tags.Where(t => serverId != null && t.ServerId == serverId))
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Code == positionTagOrCode || p.Tags.Any(t => t.Tag == positionTagOrCode));
        }

        public async Task<IReadOnlyCollection<Position>> GetAllPositionsWithTagsAsync(int? serverId)
        {
            return await _context.Positions
                .Include(p => p.Tags.Where(t => serverId != null && t.ServerId == serverId))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
