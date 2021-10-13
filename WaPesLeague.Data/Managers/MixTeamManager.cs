using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixTeamManager : IMixTeamManager
    {
        private readonly WaPesDbContext _context;
        public MixTeamManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixTeam> AddAsync(MixTeam mixTeam)
        {
            await _context.MixTeams.AddAsync(mixTeam);
            await _context.SaveChangesAsync();

            return mixTeam;
        }

        public async Task<MixTeam> UpdateAsync(MixTeam mixTeam)
        {
            var currentMixTeam = await _context.MixTeams.FindAsync(mixTeam.MixTeamId);
            if (currentMixTeam != null)
            {
                _context.Entry(currentMixTeam).CurrentValues.SetValues(mixTeam);
                await _context.SaveChangesAsync();
            }
            return currentMixTeam;
        }

        public async Task<List<MixTeam>> AddMultipleAsync(List<MixTeam> mixTeams)
        {
            await _context.MixTeams.AddRangeAsync(mixTeams);
            await _context.SaveChangesAsync();

            return mixTeams;
        }
    }
}
