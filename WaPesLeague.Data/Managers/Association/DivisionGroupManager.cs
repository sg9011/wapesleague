using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class DivisionGroupManager : IDivisionGroupManager
    {
        private readonly WaPesDbContext _context;
        public DivisionGroupManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<DivisionGroup> GetDivisionGroupByGoogleSheetImportTypeIdAsync(int GoogleSheetImportTypId)
        {
            return await _context.DivisionGroups
                .Include(dg => dg.DivisionGroupRounds)
                    .ThenInclude(dgr => dgr.Matches)
                        .ThenInclude(m => m.MatchTeams)
                            .ThenInclude(mt => mt.MatchTeamStats)
                .Include(dg => dg.DivisionGroupRounds)
                    .ThenInclude(dgr => dgr.Matches)
                        .ThenInclude(m => m.MatchTeams)
                            .ThenInclude(mt => mt.MatchTeamPlayers)
                                .ThenInclude(mtp => mtp.MatchTeamPlayerStats)
                .Include(dg => dg.DivisionGroupRounds)
                    .ThenInclude(dgr => dgr.Matches)
                        .ThenInclude(m => m.MatchTeams)
                            .ThenInclude(mt => mt.MatchTeamPlayers)
                                .ThenInclude(mtp => mtp.MatchTeamPlayerEvents)
                .AsNoTracking()
                .SingleOrDefaultAsync(dg => dg.GoogleSheetImportTypeId == GoogleSheetImportTypId);
        }
    }
}
