using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixChannelTeamManager : IMixChannelTeamManager
    {
        private readonly WaPesDbContext _context;
        public MixChannelTeamManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixChannelTeam> AddAsync(MixChannelTeam mixChannelTeam)
        {
            await _context.MixChannelTeams.AddAsync(mixChannelTeam);
            await _context.SaveChangesAsync();

            return mixChannelTeam;

        }

        public async Task<List<MixChannelTeam>> AddMultipleAsync(List<MixChannelTeam> mixChannelTeams)
        {
            await _context.MixChannelTeams.AddRangeAsync(mixChannelTeams);
            await _context.SaveChangesAsync();

            return mixChannelTeams;
        }
    }
}
