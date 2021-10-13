using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixPositionReservationManager : IMixPositionReservationManager
    {
        private readonly WaPesDbContext _context;
        public MixPositionReservationManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<MixPositionReservation> UpdateAsync(MixPositionReservation mixPositionReservation)
        {
            var currentMixPositionReservation = await _context.MixPositionReservations.FindAsync(mixPositionReservation.MixPositionReservationId);
            if (currentMixPositionReservation != null)
            {
                _context.Entry(currentMixPositionReservation).CurrentValues.SetValues(mixPositionReservation);
                await _context.SaveChangesAsync();
            }
            return currentMixPositionReservation;
        }

        public async Task<MixPositionReservation> AddAsync(MixPositionReservation mixPositionReservation)
        {
            await _context.MixPositionReservations.AddAsync(mixPositionReservation);
            await _context.SaveChangesAsync();

            return mixPositionReservation;
        }

        public async Task<MixPositionReservation> GetActiveMixPositionReservationByServerIdAndDiscordChannelIdAndUserIdAsync(int serverId, string discordChannelId, int userId)
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();

            return await _context.MixPositionReservations
                .AsNoTracking()
                .FirstOrDefaultAsync(mpr =>
                    mpr.UserId == userId
                    && mpr.DateEnd == null
                    && mpr.MixPosition.DateEnd == null
                    && mpr.MixPosition.MixTeam.PositionsLocked == false
                    && mpr.MixPosition.MixTeam.MixSession.DateClosed == null
                    && mpr.MixPosition.MixTeam.MixSession.DateToClose > dbNow
                    && mpr.MixPosition.MixTeam.MixSession.MixChannel.DiscordChannelId == discordChannelId
                    && mpr.MixPosition.MixTeam.MixSession.MixChannel.MixGroup.ServerId == serverId
                    && mpr.MixPosition.MixTeam.MixSession.MixChannel.Enabled == true
                    && mpr.MixPosition.MixTeam.MixSession.MixChannel.MixGroup.IsActive == true
                    && mpr.MixPosition.MixTeam.MixSession.MixChannel.MixGroup.Server.IsActive == true
                );
        }

        public async Task<IReadOnlyCollection<MixPositionReservation>> GetAllNonActiveReservationWithPlayTimeByUserId(int userId)
        {
            return await _context.MixPositionReservations
                .Include(mpr => mpr.MixPosition.Position.PositionGroup)
                .Include(mpr => mpr.MixPosition.MixTeam.MixSession)
                .Where(mpr => mpr.UserId == userId && mpr.DateEnd != null && mpr.DateEnd > mpr.MixPosition.MixTeam.MixSession.DateStart)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MixPositionReservation>> GetAll()
        {
            return await _context.MixPositionReservations
                .Include(mpr => mpr.MixPosition.Position.PositionGroup)
                .Include(mpr => mpr.MixPosition.MixTeam.MixSession)
                .Where(mpr => mpr.DateEnd != null && mpr.DateEnd > mpr.MixPosition.MixTeam.MixSession.DateStart)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MixPositionReservation>> GetAllReservationsForSessionsAsync(IEnumerable<int> mixSessionIds)
        {
            return await _context.MixPositionReservations
                .Include(mpr => mpr.MixPosition.Position.PositionGroup)
                .Include(mpr => mpr.MixPosition.MixTeam)
                .Where(mpr => mpr.DateEnd != null
                    && mixSessionIds.Any(msId => msId == mpr.MixPosition.MixTeam.MixSessionId))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
