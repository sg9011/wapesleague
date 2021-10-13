using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Mix;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class MixPositionManager : IMixPositionManager
    {
        private readonly WaPesDbContext _context;
        public MixPositionManager(WaPesDbContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyCollection<MixPosition>> GetAvailablePositionsForTeamAsync(int mixTeamId)
        {
            return await _context.MixPositions
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.Tags)
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.ChildPositions)
                        .ThenInclude(cp => cp.Tags)
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.ParentPosition)
                        .ThenInclude(pp => pp.Tags)
                .Where(mp => mp.MixTeamId == mixTeamId && mp.DateEnd == null && mp.Reservations.All(r => r.DateEnd != null))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MixPosition> GetActivePositionByUserIdMixTeamIdAsync(int mixTeamId, int userId)
        {
            return await _context.MixPositions
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.Tags)
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.ChildPositions)
                        .ThenInclude(cp => cp.Tags)
                .Include(mp => mp.Position)
                    .ThenInclude(p => p.ParentPosition)
                        .ThenInclude(pp => pp.Tags)
                .Where(mp => mp.MixTeamId == mixTeamId && mp.DateEnd == null && mp.Reservations.Any(r => r.DateEnd == null && r.UserId == userId))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<MixPosition> AddAsync(MixPosition mixPosition)
        {
            await _context.MixPositions.AddAsync(mixPosition);
            await _context.SaveChangesAsync();

            return mixPosition;
        }

        public async Task<List<MixPosition>> AddMultipleAsync(List<MixPosition> mixPositions)
        {
            await _context.MixPositions.AddRangeAsync(mixPositions);
            await _context.SaveChangesAsync();

            return mixPositions;
        }

        public async Task<MixPosition> UpdateAsync(MixPosition mixPosition)
        {
            var currentMixPosition = await _context.MixPositions.FindAsync(mixPosition.MixPositionId);
            if (currentMixPosition != null)
            {
                _context.Entry(currentMixPosition).CurrentValues.SetValues(mixPosition);
                await _context.SaveChangesAsync();
            }
            return currentMixPosition;
        }

        public async Task<IReadOnlyCollection<MixPosition>> GetActivePositionsByDiscordIdsAndTeamAndPositionAsync(string discordServerId, string discordChannelId, string team, string position)
        {
            return await _context.MixPositions
                .Include(mp => mp.Position)
                    .ThenInclude(mp => mp.Tags)
                .Include(mp => mp.Reservations.Where(r => r.DateEnd == null))
                .AsNoTracking()
                .Where(mp =>
                    mp.MixTeam.MixSession.MixChannel.MixGroup.Server.IsActive == true
                    && mp.MixTeam.MixSession.MixChannel.MixGroup.Server.DiscordServerId == discordServerId
                    && mp.MixTeam.MixSession.MixChannel.MixGroup.IsActive
                    && mp.MixTeam.MixSession.MixChannel.Enabled == true
                    && mp.MixTeam.MixSession.MixChannel.DiscordChannelId == discordChannelId
                    && mp.MixTeam.MixSession.DateClosed == null
                    && mp.MixTeam.PositionsLocked == false
                    && mp.DateEnd == null
                    && (mp.MixTeam.Name == team || mp.MixTeam.Tags.Any(mtt => mtt.Tag == team))
                    && (mp.Position.Code == position || mp.Position.Tags.Any(mpt => mpt.Tag == position))
                    )
                .ToListAsync();
        }
    }
}
