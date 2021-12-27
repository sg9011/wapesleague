using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class DivisionGroupRoundManager : IDivisionGroupRoundManager
    {
        private readonly WaPesDbContext _context;
        public DivisionGroupRoundManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task DeleteByDivisionGroupIdAsync(int divisionGroupId)
        {
            var divisionGroupRoundsToDelete = await _context.DivisionGroupRounds
                .Where(dgr => dgr.DivisionGroupId == divisionGroupId)
                .ToListAsync();

            _context.DivisionGroupRounds.RemoveRange(divisionGroupRoundsToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
