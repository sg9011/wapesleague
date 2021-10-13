using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class PositionTagManager : IPositionTagManager
    {
        private readonly WaPesDbContext _context;
        public PositionTagManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<PositionTag> AddAsync(PositionTag positionTag)
        {
            await _context.PositionTags.AddAsync(positionTag);
            await _context.SaveChangesAsync();

            return positionTag;
        }

        public async Task<PositionTag> UpdateAsync(PositionTag positionTag)
        {
            var currentPositionTag = await _context.PositionTags.FindAsync(positionTag.PositionTagId);
            if (currentPositionTag != null)
            {
                _context.Entry(currentPositionTag).CurrentValues.SetValues(positionTag);
                await _context.SaveChangesAsync();
            }
            return currentPositionTag;
        }

        public async Task DeleteAsync(int positionTagId)
        {
            var currentPositionTag = await _context.PositionTags.FindAsync(positionTagId);
            if (currentPositionTag != null)
            {
                _context.PositionTags.Remove(currentPositionTag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
