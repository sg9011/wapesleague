using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixChannelManager : IMixChannelManager
    {
        private readonly WaPesDbContext _context;
        public MixChannelManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetActiveMixChannelIdByDiscordIds(string discordServerId, string discordChannelId)
        {
            return await _context.MixChannels
                .AsNoTracking()
                .Where(mc => mc.DiscordChannelId == discordChannelId && mc.MixGroup.Server.DiscordServerId == discordServerId && mc.MixGroup.Server.IsActive && mc.MixGroup.IsActive && mc.Enabled)
                .Select(mc => mc.MixChannelId)
                .SingleOrDefaultAsync();
        }

        public async Task<MixChannel> GetActiveMixChannelByDiscordIds(string discordServerId, string discordChannelId)
        {
            return await _context.MixChannels
                .AsNoTracking()
                .Where(mc => mc.DiscordChannelId == discordChannelId && mc.MixGroup.Server.DiscordServerId == discordServerId && mc.MixGroup.Server.IsActive && mc.MixGroup.IsActive && mc.Enabled)
                .SingleOrDefaultAsync();
        }


        public async Task<MixChannel> GetActiveMixChannelByDiscordChannelIdAndInternalServerId(int serverId, string discordChannelId)
        {
            return await _context.MixChannels
                .Include(mc => mc.MixGroup)
                    .ThenInclude(mg => mg.Server)
                .Include(mc => mc.MixSessions)
                .Include(mc => mc.MixChannelTeams)
                    .ThenInclude(mct => mct.DefaultFormation)
                        .ThenInclude(mctp => mctp.Position)
                            .ThenInclude(p => p.PositionGroup)
                .Include(mc => mc.MixChannelTeams)
                    .ThenInclude(mct => mct.Tags)
                    .AsNoTracking()
                .SingleOrDefaultAsync(mc => 
                    mc.DiscordChannelId == discordChannelId
                    && mc.MixGroup.ServerId == serverId
                    && mc.MixGroup.Server.IsActive == true
                    && mc.MixGroup.IsActive == true
                    && mc.Enabled == true);
        }

        public async Task<bool> HasActiveMixChannelByDiscordIdsAsync(string discordServerId, string discordChannelId)
        {
            return await _context.MixChannels
                .AnyAsync(mc => mc.DiscordChannelId == discordChannelId 
                    && mc.MixGroup.Server.DiscordServerId == discordServerId
                    && mc.MixGroup.Server.IsActive == true
                    && mc.MixGroup.IsActive == true
                    && mc.Enabled == true
                    && mc.MixSessions.Any(ms => ms.DateClosed == null));
        }
        public async Task<bool> HasActiveMixChannelByDiscordIdsAndTeamAndPositionAsync(string discordServerId, string discordChannelId, string team, string position)
        {
            return await _context.MixChannels
                .AnyAsync(mc => 
                    mc.DiscordChannelId == discordChannelId
                    && mc.MixGroup.Server.DiscordServerId == discordServerId
                    && mc.MixGroup.Server.IsActive == true
                    && mc.MixGroup.IsActive == true
                    && mc.Enabled == true
                    && mc.MixSessions.Any(ms => 
                        ms.DateClosed == null
                        && ms.MixTeams.Any(mt => 
                            mt.PositionsLocked == false
                            && (mt.Name == team || mt.Tags.Any(t => t.Tag == team))
                            && mt.Formation.Any(mp => mp.DateEnd == null && (mp.Position.Code == position || mp.Position.Tags.Any(t => t.Tag == position)))
                        )
                    )
                );
        }

        public async Task<bool> UserCanSignIntoMixChannelAsync(string discordServerId, string discordChannelId, int userId)
        {
            var mixGroupId = (await _context.MixChannels
                .AsNoTracking()
                .FirstOrDefaultAsync(mc => 
                    mc.DiscordChannelId == discordChannelId
                    && mc.MixGroup.IsActive == true
                    && mc.MixGroup.Server.DiscordServerId == discordServerId
                    && mc.MixGroup.Server.IsActive == true)
                )?.MixGroupId;

            if (mixGroupId == null)
                return false;

            return !await _context.MixGroups
                .Where(mixGroup => mixGroup.MixGroupId == mixGroupId)
                .AnyAsync(mixGroup => mixGroup.MixChannels
                    .Any(mixChannel => mixChannel.Enabled == true && mixChannel.DiscordChannelId != discordChannelId && mixChannel.MixSessions
                        .Any(mixSession => mixSession.DateClosed == null && mixSession.MixTeams
                            .Any(mixTeam => mixTeam.PositionsLocked == false && mixTeam.Formation
                                .Any(position => position.DateEnd == null && position.Reservations
                                    .Any(positionReservation => positionReservation.DateEnd == null && positionReservation.UserId == userId)
                                    )
                                )
                            )
                        )
                    );

        }

        public async Task<MixChannel> UpdateAsync(MixChannel mixChannel)
        {
            var currentMixChannel = await _context.MixChannels.FindAsync(mixChannel.MixChannelId);
            if (currentMixChannel != null)
            {
                _context.Entry(currentMixChannel).CurrentValues.SetValues(mixChannel);
                await _context.SaveChangesAsync();
            }
            return currentMixChannel;
        }

        public async Task<MixChannel> AddAsync(MixChannel mixChannel)
        {
            await _context.MixChannels.AddAsync(mixChannel);
            await _context.SaveChangesAsync();

            return mixChannel;
        }

        public async Task<bool> DisableChannelAsync(int mixChannelId)
        {
            var currentMixChannel = await _context.MixChannels.FindAsync(mixChannelId);
            if (currentMixChannel != null)
            {
                currentMixChannel.Enabled = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
