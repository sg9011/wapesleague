using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WaPesLeague.Data.Entities.Discord;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Data.Managers
{
    public class ServerManager : IServerManager
    {
        private readonly WaPesDbContext _context;
        public ServerManager(WaPesDbContext context)
        {
            _context = context;
        }
        public async Task<Server> AddAsync(Server server)
        {
            await _context.Servers.AddAsync(server);
            await _context.SaveChangesAsync();

            return server;
        }

        public async Task<Server> GetServerByDiscordIdAsync(string discordServerId)
        {
            return await _context.Servers
                .Include(s => s.DefaultTeams)
                    .ThenInclude(s => s.Tags)
                .Include(s => s.ServerFormations)
                    .ThenInclude(sf => sf.Tags)
                .Include(s => s.ServerFormations)
                    .ThenInclude(sf => sf.Positions)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.DiscordServerId == discordServerId && s.IsActive == true);
        }

        public async Task<int?> GetServerIdByDiscordServerId(ulong discordServerId)
        {
            var serverIds = await _context.Servers
                .Where(s => s.DiscordServerId == discordServerId.ToString() && s.IsActive == true)
                .AsNoTracking()
                .Select(s => s.ServerId)
                .ToListAsync();

            if (serverIds.Any())
                return serverIds.First();
            return null;
        }

        public async Task<Server> UpdateAsync(Server server)
        {
            var currentServer = await _context.Servers.FindAsync(server.ServerId);
            if (currentServer != null)
            {
                _context.Entry(currentServer).CurrentValues.SetValues(server);
                await _context.SaveChangesAsync();
            }
            return currentServer;
        }

        public async Task<bool> DeActivateServerAsync(int server)
        {
            var currentServer = await _context.Servers.FindAsync(server);
            if (currentServer != null)
            {
                currentServer.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
