using System.Collections.Generic;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixChannelTeamTagManager : IMixChannelTeamTagManager
    {
        private readonly WaPesDbContext _context;
        public MixChannelTeamTagManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixChannelTeamTag> AddAsync(MixChannelTeamTag mixChannelTeamTag)
        {
            await _context.MixChannelTeamTags.AddAsync(mixChannelTeamTag);
            await _context.SaveChangesAsync();

            return mixChannelTeamTag;

        }

        public async Task<List<MixChannelTeamTag>> AddMultipleAsync(List<MixChannelTeamTag> mixChannelTeamTags)
        {
            await _context.MixChannelTeamTags.AddRangeAsync(mixChannelTeamTags);
            await _context.SaveChangesAsync();

            return mixChannelTeamTags;
        }

    }
}
