using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixGroupManager : IMixGroupManager
    {
        private readonly WaPesDbContext _context;
        public MixGroupManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<MixGroup> AddAsync(MixGroup mixGroup)
        {
            await _context.MixGroups.AddAsync(mixGroup);
            await _context.SaveChangesAsync();

            return mixGroup;
        }

        public async Task<MixGroup> GetActiveMixGroupByDiscordServerAndChannelIdAsync(string serverId, string channelId)
        {
            return await _context.MixGroups
                .Include(mg => mg.MixChannels)
                    .ThenInclude(mc => mc.MixSessions)
                .Include(mg => mg.MixGroupRoleOpenings.Where(mgro => mgro.IsActive == true))
                .AsNoTracking()
                .SingleOrDefaultAsync(mg => mg.Server.DiscordServerId == serverId && mg.Server.IsActive && mg.IsActive == true && mg.MixChannels.Any(mc => mc.DiscordChannelId == channelId));
        }

        public async Task<MixGroup> GetMixGroupByIdAsync(int mixGroupId)
        {
            return await _context.MixGroups
                .Include(mg => mg.MixChannels)
                    .ThenInclude(mc => mc.MixSessions)
                .Include(mg=> mg.MixChannels)
                    .ThenInclude(mc => mc.MixChannelTeams)
                        .ThenInclude(mt => mt.Tags)
                 .Include(mg => mg.MixChannels)
                    .ThenInclude(mc => mc.MixChannelTeams)
                        .ThenInclude(mt => mt.DefaultFormation)
                .AsNoTracking()
                .SingleOrDefaultAsync(mg => mg.MixGroupId == mixGroupId);
        }

        public async Task<bool> DeActivateMixGroupAsync(int mixGroup)
        {
            var currentMixGroup = await _context.MixGroups.FindAsync(mixGroup);
            if (currentMixGroup != null)
            {
                currentMixGroup.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HasActiveMixChannelsAsync(int mixGroupId)
        {
            return await _context.MixChannels.AnyAsync(mc => mc.Enabled == true && mc.MixGroupId == mixGroupId);
        }
    }
}
