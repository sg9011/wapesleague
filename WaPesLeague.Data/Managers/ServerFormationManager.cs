using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerFormationManager : IServerFormationManager
    {
        private readonly WaPesDbContext _context;

        public ServerFormationManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<ServerFormation> AddAsync(ServerFormation serverFormation)
        {
            await _context.ServerFormations.AddAsync(serverFormation);
            await _context.SaveChangesAsync();

            return serverFormation;
        }

        public async Task<ServerFormation> UpdateAsync(ServerFormation serverFormation)
        {
            var currentServerFormation = await _context.ServerFormations.FindAsync(serverFormation.ServerFormationId);
            if (currentServerFormation != null)
            {
                _context.Entry(currentServerFormation).CurrentValues.SetValues(serverFormation);
                await _context.SaveChangesAsync();
            }
            return currentServerFormation;
        }

        public async Task<bool> HasFormationWithTagOrNameAsync(string tagOrName, string discordServerId)
        {
            return await _context.ServerFormations
                .AnyAsync(f => f.Server.DiscordServerId == discordServerId && (f.Name == tagOrName || f.Tags.Any(t => t.Tag == tagOrName)));
        }

        public async Task<ServerFormation> GetDefaultFormationAsync(int serverId)
        {
            return await _context.ServerFormations
                .Include(f => f.Tags)
                .Include(f => f.Positions)
                .AsNoTracking()
                .SingleAsync(f => f.ServerId == serverId && f.IsDefault == true);
        }

        public async Task<ServerFormation> GetFormationByTagOrNameAsync(string tagOrName, int serverId)
        {
            return await _context.ServerFormations
                .Include(f => f.Tags)
                .Include(f => f.Positions)
                .AsNoTracking()
                .SingleOrDefaultAsync(f => f.ServerId == serverId && (f.Name == tagOrName || f.Tags.Any(t => t.Tag == tagOrName)));
        }

        public async Task<IReadOnlyCollection<ServerFormation>> GetAllFormationsByDiscordServerIdAsync(string discordServerId)
        {
            return await _context.ServerFormations
                .Include(f => f.Tags)
                .Include(f => f.Positions)
                    .ThenInclude(fp => fp.Position)
                        .ThenInclude(p => p.PositionGroup)
                .Where(sf => sf.Server.DiscordServerId == discordServerId && sf.Server.IsActive == true) 
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
