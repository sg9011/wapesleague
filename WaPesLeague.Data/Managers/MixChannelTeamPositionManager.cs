using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixChannelTeamPositionManager : IMixChannelTeamPositionManager
    {
        private readonly WaPesDbContext _context;
        public MixChannelTeamPositionManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixChannelTeamPosition> AddAsync(MixChannelTeamPosition mixChannelTeamPosition)
        {
            await _context.MixChannelTeamPositions.AddAsync(mixChannelTeamPosition);
            await _context.SaveChangesAsync();

            return mixChannelTeamPosition;
        }

        public async Task<List<MixChannelTeamPosition>> AddMultipleAsync(List<MixChannelTeamPosition> mixChannelTeamPositions)
        {
            await _context.MixChannelTeamPositions.AddRangeAsync(mixChannelTeamPositions);
            await _context.SaveChangesAsync();

            return mixChannelTeamPositions;
        }
    }
}
