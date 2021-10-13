using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixSessionManager : IMixSessionManager
    {
        private readonly WaPesDbContext _context;
        public MixSessionManager(WaPesDbContext context)
        {
            _context = context;
        }

        public async Task<MixSession> AddAsync(MixSession mixSession)
        {
            await _context.MixSessions.AddAsync(mixSession);
            await _context.SaveChangesAsync();

            return mixSession;
        }

        public async Task<MixSession> UpdateAsync(MixSession mixSession)
        {
            var currentMixSession = await _context.MixSessions.FindAsync(mixSession.MixSessionId);
            if (currentMixSession != null)
            {
                _context.Entry(currentMixSession).CurrentValues.SetValues(mixSession);
                await _context.SaveChangesAsync();
            }
            return currentMixSession;
        }

        public async Task<MixSession> GetCurrentMixSessionByChannelIdAsync(int mixChannelId)
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();

            return await _context.MixSessions
                .Include(ms => ms.MixChannel)
                    .ThenInclude(mc => mc.MixGroup)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Reservations.Where(f => f.DateEnd == null))
                            .ThenInclude(r => r.User)
                                .ThenInclude(u => u.PlatformUsers)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Position)
                            .ThenInclude(p => p.PositionGroup)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Tags)
                .AsNoTracking()
                .SingleOrDefaultAsync(ms => ms.MixChannelId == mixChannelId
                    && ms.DateClosed == null
                    && ms.DateToClose > dbNow
                    && ms.MixChannel.MixGroup.IsActive == true
                    && ms.MixChannel.MixGroup.Server.IsActive == true);
        }

        public async Task<MixSession> GetMixSessionByIdAsync(int mixSessionId, int serverId)
        {
            return await _context.MixSessions
                .Include(ms => ms.MixChannel)
                    .ThenInclude(mc => mc.MixGroup)
                        .ThenInclude(mg => mg.Server)
                .Include(ms => ms.RoomOwner)
                    .ThenInclude(ro => ro.PlatformUsers)
                        .ThenInclude(pu => pu.Platform)
                .Include(ms => ms.RoomOwner)
                    .ThenInclude(ro => ro.UserMembers.Where(um => um.ServerId == serverId))
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Reservations.Where(f => f.DateEnd == null))
                            .ThenInclude(r => r.User)
                                .ThenInclude(u => u.PlatformUsers)
                                    .ThenInclude(u => u.Platform)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Reservations.Where(f => f.DateEnd == null))
                            .ThenInclude(r => r.User)
                                .ThenInclude(u => u.UserMembers)
                 .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Tags)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Position)
                            .ThenInclude(p => p.PositionGroup)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Position)
                            .ThenInclude(p => p.Tags.Where(t => t.ServerId == serverId && t.IsDisplayValue == true))
                .AsNoTracking()
                .SingleOrDefaultAsync(ms => ms.MixSessionId == mixSessionId);
        }

        public async Task<MixSession> GetActiveMixSessionByServerIdAndDiscordChannelIdAsync(int serverId, string discordchannelId)
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();

            return await _context.MixSessions
                .Include(ms => ms.MixChannel.MixGroup.Server)
                .Include(ms => ms.RoomOwner.PlatformUsers)
                        .ThenInclude(pu => pu.Platform)
                .Include(ms => ms.RoomOwner.UserMembers.Where(um => um.ServerId == serverId))
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Reservations.Where(f => f.DateEnd == null))
                            .ThenInclude(r => r.User.PlatformUsers)
                                .ThenInclude(u => u.Platform)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Reservations.Where(f => f.DateEnd == null))
                            .ThenInclude(r => r.User.UserMembers.Where(um => um.ServerId == serverId))
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Position.PositionGroup)
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation.Where(f => f.DateEnd == null))
                        .ThenInclude(f => f.Position)
                            .ThenInclude(p => p.Tags.Where(t => t.ServerId == serverId && t.IsDisplayValue == true))
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Tags)
                .AsNoTracking()
                .SingleOrDefaultAsync(ms => ms.MixChannel.DiscordChannelId == discordchannelId
                    && ms.MixChannel.MixGroup.ServerId == serverId
                    && ms.MixChannel.MixGroup.Server.IsActive == true
                    && ms.DateClosed == null
                    && ms.DateToClose > dbNow
                    && ms.MixChannel.MixGroup.IsActive == true);
        }

        public async Task<bool> HasOpenMixSessionByDiscordIds(string discordServerid, string discordChannelId, DateTime time)
        {
            return await _context.MixSessions
                .AnyAsync(ms => 
                    ms.MixChannel.DiscordChannelId == discordChannelId
                    && ms.MixChannel.MixGroup.Server.DiscordServerId == discordServerid
                    && ms.MixChannel.MixGroup.Server.IsActive == true
                    && ms.MixChannel.MixGroup.IsActive
                    && ms.MixChannel.Enabled == true
                    && ms.DateRegistrationOpening < time
                    && ms.DateToClose > time &&
                    ms.DateClosed == null);
        }

        public async Task<bool> CheckIfExtraMixSessionShouldBeCreatedAsync(int mixGroupId)
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();

            return await _context.MixSessions.
                Where(x => x.MixChannel.MixGroupId == mixGroupId
                    && x.DateClosed == null
                    && x.DateToClose > dbNow
                    && x.MixChannel.MixGroup.CreateExtraMixChannels == true
                    && x.MixChannel.Enabled == true)
                .AllAsync(ms => 
                    //Has to be true for each item: Team is Locked Or Position Is Not tRequired For Mix Or (There have to be reservations and all of them should be ongoing)
                    ms.MixTeams.All(mt => 
                        mt.PositionsLocked == true //PosLocked
                        || mt.Formation.All(mp => // or All pos are NOT required 
                            mp.Position.IsRequiredForMix == false
                            || mp.DateEnd != null // mixpos NOT closed
                            || mp.Reservations.Any(r => r.DateEnd == null) //Or Any ongoing Reservation for this required open position
                        )
                    )
                );
        }

        public async Task<IReadOnlyCollection<MixSession>> GetMixSessionsToCloseAsync()
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();
            return await _context.MixSessions
                .Include(ms => ms.MixChannel.MixGroup.Server)
                .Where(ms => ms.DateClosed == null && ms.DateToClose < dbNow)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<EndDbSessionResult> EndCurrentSessionAsync(int mixSessionId)
        {
            var dbNow = DateTimeHelper.GetDatabaseNow();
            var session = await _context.MixSessions
                    .Include(ms => ms.MixTeams)
                        .ThenInclude(mt => mt.Formation)
                            .ThenInclude(f => f.Reservations)
                .SingleOrDefaultAsync(ms => ms.MixSessionId == mixSessionId);

            if (session == null)
                return new EndDbSessionResult(false);

            session.DateClosed = dbNow;
            session.DateLastUpdated = dbNow;
            var lastSignOutDateInSession = session.MixTeams
                .SelectMany(mt => mt.Formation
                    .SelectMany(mp => mp.Reservations
                        .Select(r => r.DateEnd ?? DateTime.MinValue)
                    )
                )
                .DefaultIfEmpty(dbNow)
                .Max();

            var signedOutUserIds = new List<int>();
            foreach (var mixTeam in session.MixTeams.Where(mt => mt.PositionsLocked == false))
            {
                foreach(var mixPosition in mixTeam.Formation.Where(mp => mp.DateEnd == null))
                {
                    mixPosition.DateEnd = dbNow; //Do we want to close the last active position in a mixTeam or will this make future queriyng more difficult;
                    foreach(var reservation in mixPosition.Reservations.Where(r => r.DateEnd == null))
                    {
                        if (reservation.DateStart < lastSignOutDateInSession)
                            reservation.DateEnd = lastSignOutDateInSession;
                        else
                            reservation.DateEnd = reservation.DateStart;
                        signedOutUserIds.Add(reservation.UserId);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return new EndDbSessionResult(true, signedOutUserIds);
        }

        public async Task<IReadOnlyCollection<MixSession>> GetActiveMixSessionsByMixGroupIdAsync(int mixGroupId)
        {
            return await _context.MixSessions
                .Where(ms => 
                    ms.MixChannel.MixGroupId == mixGroupId
                    && ms.MixChannel.Enabled == true
                    && ms.DateClosed == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MixSession>> GetMixSessionsThatWillStartBetweenDatesAsync(DateTime rangeStartTime, DateTime rangeEndTime)
        {
            return await _context.MixSessions
                .Include(ms => ms.MixTeams.Where(mt => mt.PositionsLocked == false))
                    .ThenInclude(mt => mt.Formation)
                        .ThenInclude(f => f.Reservations.Where(r => r.DateEnd == null))
                            .ThenInclude(r => r.User.UserMembers)
                .Include(ms => ms.MixChannel.MixGroup.Server)
                .Where(ms => 
                    ms.DateClosed == null
                    && ms.DateStart >= rangeStartTime
                    && ms.DateStart < rangeEndTime
                    && ms.MixChannel.Enabled
                    && ms.MixChannel.MixGroup.IsActive
                    && ms.MixChannel.MixGroup.Server.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MixSession>> GetMixSessionAndTeamsBySessionIdsAsync(IEnumerable<int> mixSessionIds)
        {
            return await _context.MixSessions
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation)
                .Where(x => mixSessionIds.Any(i => i == x.MixSessionId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<MixSession>> GetSessionsToCalculateStatsAsync()
        {
            return await _context.MixSessions
                .Include(ms => ms.MixTeams)
                    .ThenInclude(mt => mt.Formation)
                .Where(ms => ms.DateClosed != null && ms.DateStatsCalculated == null)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SetStatsCalculatedDateForMixSessionsAsync(IEnumerable<int> mixSessionIds)
        {
            var now = DateTimeHelper.GetDatabaseNow();
            var mixSessions = await _context.MixSessions.
                Where(ms => mixSessionIds.Any(msId => msId == ms.MixSessionId))
                .ToListAsync();

            mixSessions.ForEach(ms => ms.DateStatsCalculated = now);

            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<int, int>> GetAllSessionIdsWithServerId()
        {
            return await _context.MixSessions
                .Include(ms => ms.MixChannel.MixGroup)
                .AsNoTracking()
                .ToDictionaryAsync(x => x.MixSessionId, y => y.MixChannel.MixGroup.ServerId);
        }
    }
}
