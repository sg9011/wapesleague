using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Association;
using WaPesLeague.Data.Managers.Association.Interfaces;

namespace WaPesLeague.Data.Managers.Association
{
    public class AssociationTeamManager : IAssociationTeamManager
    {
        private readonly WaPesDbContext _context;
        public AssociationTeamManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<AssociationTeam> AddAsync(AssociationTeam accociationTeam)
        {
            await _context.AssociationTeams.AddAsync(accociationTeam);
            await _context.SaveChangesAsync();

            return accociationTeam;
        }

        public async Task<List<AssociationTeam>> AddMultipleAsync(List<AssociationTeam> associationTeamsToAdd)
        {
            await _context.AssociationTeams.AddRangeAsync(associationTeamsToAdd);
            await _context.SaveChangesAsync();

            return associationTeamsToAdd;
        }

        public async Task<IReadOnlyCollection<AssociationTeam>> GetAssociationTeamsByAssociationIdAsync(int associationId)
        {
            return await _context.AssociationTeams
                .Include(at => at.MatchTeams)
                    .ThenInclude(mt => mt.Match)
                        .ThenInclude(m => m.DivisionGroupRound)
                .Where(at => at.AssociationId == associationId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
