using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class FormationManager : IFormationManager
    {
        private readonly WaPesDbContext _context;

        public FormationManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Formation>> GetAllFormationsAsync()
        {
            return await _context.Formations
                .Include(f => f.Tags)
                .Include(f => f.FormationPositions)
                    .ThenInclude(fp => fp.Position)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
